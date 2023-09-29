﻿using Newtonsoft.Json;
using Project.Web.Models;
using Project.Web.Service.IService;
using Project.Web.Utility;
using System.Net;
using System.Text;

namespace Project.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {

            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ProjectAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                //token

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (requestDto.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSucess = false, Message = "Not found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSucess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSucess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSucess = false, Message = "Internal Server Error" };
                    case HttpStatusCode.BadRequest:
                        //return new() { IsSucess = false, Message = "Bad Request" };   
                        return new() { IsSucess = false, Message = apiResponseDto.Message };
                    default:
                        //var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        //var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            } catch (Exception ex)
            {
                var dto = new ResponseDto()
                {
                    Message = ex.Message,
                    IsSucess = false
                };
                return dto;
            }
        }
    }
}
