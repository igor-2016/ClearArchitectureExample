using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.Net.Mime;
using System.Text;
using Utils.Sys.Exceptions;
using Utils.Sys.Extensions;
using Utils.Sys.RichHttpClient.Exceptions;

namespace Utils.Sys.RichHttpClient.Extensions
{
    public static class HttpClientExtensions
    {
        public sealed class Unit
        {
            public static Unit Value
            {
                get
                {
                    return new Unit();
                }
            }
        }

     
        public static Task<TResponseModel> SafeGetAsync<TResponseModel, TErrorModel>(
            this HttpClient httpClient,
            string resource,
            CancellationToken cancellationToken,
            Encoding? encoding = null,
            ContentType? contentType  = null,
            object? urlParams = null,
            object? headers = null)
            where TResponseModel : class
            where TErrorModel : class
        {
            contentType = contentType.DefaultIfNull(ContentTypeProvider.Json);
            encoding = encoding ?? Encoding.UTF8;
            return CallAsync<Unit, TResponseModel, TErrorModel>(httpClient, HttpMethod.Get, resource, Unit.Value, 
                urlParams, headers, cancellationToken, contentType, encoding);
        }

        public static Task<TResponseModel> SafePostAsync<TRequestModel, TResponseModel, TErrorModel>(
            this HttpClient httpClient,
            string resource,
            TRequestModel requestModel,
            CancellationToken cancellationToken,
            Encoding? encoding = null,
            ContentType? contentType = null,
            object? urlParams = null,
            object? headers = null)
            where TResponseModel : class
            where TErrorModel : class
        {
            contentType = contentType.DefaultIfNull(ContentTypeProvider.Json);
            encoding = encoding ?? Encoding.UTF8;
            return CallAsync<TRequestModel, TResponseModel, TErrorModel>(httpClient, HttpMethod.Post, resource, 
                requestModel, urlParams, headers, cancellationToken, contentType, encoding);
        }

        public static async Task SafePostAsync<TRequestModel, TErrorModel>(
            this HttpClient httpClient,
            string resource,
            TRequestModel requestModel,
            CancellationToken cancellationToken,
            Encoding? encoding = null,
            ContentType? contentType = null,
            object? urlParams = null,
            object? headers = null)
            where TErrorModel : class
        {
            contentType = contentType.DefaultIfNull(ContentTypeProvider.Json);
            encoding = encoding ?? Encoding.UTF8;
            await CallAsync<TRequestModel, Unit, TErrorModel>(httpClient, HttpMethod.Post, resource, requestModel,
                urlParams, headers, cancellationToken, contentType, encoding);
        }

        public static Task<TResponseModel> SafePostNoBodyAsync<TResponseModel, TErrorModel>(
            this HttpClient httpClient,
            string resource,
            CancellationToken cancellationToken,
            Encoding? encoding = null,
            ContentType? contentType = null,
            object? urlParams = null,
            object? headers = null)
            where TResponseModel : class
            where TErrorModel : class
        {
            contentType = contentType.DefaultIfNull(ContentTypeProvider.Json);
            encoding = encoding ?? Encoding.UTF8;
            return CallAsync<Unit, TResponseModel, TErrorModel>(httpClient, HttpMethod.Post, resource,
                null, urlParams, headers, cancellationToken, contentType, encoding);
        }


        private static async Task<TResponseModel> CallAsync<TRequestModel, TResponseModel, TErrorModel>(
            HttpClient httpClient,
            HttpMethod method,
            string resource,
            TRequestModel? requestModel,
            object? urlParams,
            object? headers,
            CancellationToken cancellationToken,
            ContentType contentType,
            Encoding encoding)
            where TResponseModel : class
            where TErrorModel : class
        {
            var request = new HttpRequestMessage
            {
                Method = method ?? HttpMethod.Get,
                RequestUri = GetRequestUriWithParams(resource, urlParams) // может запрос уже быть в строке без urlParams!!!
            };

            AddHeaders(headers, request, contentType);

            if (request.Method == HttpMethod.Post && typeof(TRequestModel) != typeof(Unit))
            {
                AddBody(requestModel, request, contentType, encoding);
            }

            TResponseModel? responseModel;

            using (request)
            {
                try
                {
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken))
                    {
                        if (response == null)
                        {
                            throw new InvalidResponseModelException("Ответ пуст!");
                        }

                        var outPutContentType = response.Content?.Headers?.ContentType?.MediaType ?? contentType.MediaType;
                        
                        responseModel = await HandleResponseAsync<TResponseModel, TErrorModel>(response,
                            new ContentType(outPutContentType), cancellationToken);
                    }
                }
                catch(HttpRequestException ex)
                {
                    throw new RichHttpClientException(ex.StatusCode, ex.Message, ex);
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
            return responseModel;
        }


       
        private static async Task<TResponseModel> HandleResponseAsync<TResponseModel, TErrorModel>(HttpResponseMessage response,
           ContentType contentType, CancellationToken cancellationToken)
           where TResponseModel : class
           where TErrorModel : class
        {

            if (response.IsSuccessStatusCode)
            {
                string? stringResponse = default;
                try
                {
                    stringResponse = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (string.IsNullOrWhiteSpace(stringResponse))
                    {
                        return null;
                    }

                    var result = DeserializeResponse<TResponseModel>(contentType, stringResponse);

                    if (result == default(TResponseModel))
                        throw new InvalidResponseModelException(stringResponse);
                    
                    return result;
                }
                catch (Exception ex)
                {
                    throw new RichHttpClientException(response.StatusCode, stringResponse, ex);
                }
            }
            else // error
            {
                string? stringResponseContent = default;
                try
                {
                    stringResponseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (typeof(TErrorModel) == typeof(Unit))
                    {
                        throw new RichHttpClientException(response.StatusCode, "Модель ошибки не указана, как есть");
                    }
                }
                catch (Exception ex)
                {
                    var readResponseException = new ReadResponseException(ex);
                    throw new RichHttpClientException(response.StatusCode, stringResponseContent, readResponseException);
                }

                TErrorModel? errorModel;
                try
                {
                    errorModel = DeserializeResponse<TErrorModel>(contentType, stringResponseContent);

                    if(errorModel == default(TErrorModel)) 
                        throw new InvalidErrorModelException($"Invalid error model {typeof(TErrorModel)}");
                }
                catch (Exception ex)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new NotFoundException(stringResponseContent, ex);
                    }

                    if (HttpStatusCodeHelper.IsSuccessStatusCode(response.StatusCode))
                    {
                        throw new HttpClientErrorException(response.StatusCode, ex);
                    }

                    if (HttpStatusCodeHelper.IsServerStatusCode(response.StatusCode))
                    {
                        throw new HttpServerErrorException(response.StatusCode, ex);
                    }

                    throw new RichHttpClientException(response.StatusCode, stringResponseContent, ex);
                }

                throw new PresentationException<TErrorModel>(response.StatusCode, errorModel);

            } // end error
        }

        private static T? DeserializeResponse<T>(ContentType contentType, string stringResponse)
           where T : class
        {
            if ((contentType.MediaType == ContentTypeProvider.Json.MediaType))
            {
                return stringResponse.DeserializeJson<T>();
            }

            if (contentType.MediaType == ContentTypeProvider.Xml.MediaType)
            {
                return stringResponse.DeserializeXml<T>();
            }

            return stringResponse as T;
        }

        private static void AddBody<TRequestModel>(TRequestModel requestModel, HttpRequestMessage request, ContentType contentType,
            Encoding encoding)
        {
            if (requestModel != null)
            {
                contentType = contentType.DefaultIfNull(ContentTypeProvider.Json);

                if (contentType.Equals(ContentTypeProvider.Json))
                {
                    var strBody = requestModel.SerializeJson(Formatting.None);
                    request.Content = new StringContent(strBody, encoding, mediaType: contentType.MediaType); //UTF8 ??
                }
                else if (contentType.Equals(ContentTypeProvider.Xml))
                {
                    var content = string.Empty;
                    if(encoding == Encoding.UTF8)
                    {
                        content = requestModel.ToXmlUtf8();
                    }
                    else
                    {
                        content = requestModel.SerializationXml();
                    }
                    request.Content = new StringContent(content, encoding, contentType.MediaType);
                }
                else
                {
                    throw new NotImplementedException($"Тип контента {contentType.MediaType} не поддерживает в данный момент.");
                }
            }
        }

        private static Uri GetRequestUriWithParams(string resource, object? urlParams)
        {

            resource = resource.Trim('/');

            var queryString =
                urlParams
                ?.SerializeJson()
                .DeserializeJson<Dictionary<string, object>>()
                .SelectMany(x =>
                {
                    switch (x.Value)
                    {
                        case string str:
                            {
                                return new[] { new { x.Key, Value = str } };
                            }

                        case IEnumerable arr:
                            {
                                return arr.Cast<object>().Select(obj => new { x.Key, Value = obj.ToString() });
                            }

                        default:
                            {
                                return new[] { new { x.Key, Value = x.Value?.ToString() ?? string.Empty } };
                            }
                    }
                })
                .Aggregate(new StringBuilder(), (builder, pair) => builder.AppendFormat("&{0}={1}", pair.Key.UrlEncode(), pair.Value.UrlEncode()))
                .ToString()
                .Substring(1);

            if (!string.IsNullOrEmpty(queryString))
            {
                var delimiter = resource.Contains('?') ? "&" : "?";
                resource = resource + delimiter + queryString;
            }

            return new Uri(resource, UriKind.Relative);
        }

        private static void AddHeaders(object? headers, HttpRequestMessage request, ContentType contentType)
        {
            request.Headers.Add("Accept", contentType.MediaType);

            if (headers != null)
            {
                foreach (var (name, value) in headers.AsDictionary())
                {
                    request.Headers.Add(name, value);
                }
            }
        }

    }
}
