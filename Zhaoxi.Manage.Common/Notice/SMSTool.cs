using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Zhaoxi.Manage.Common.Notice
{
    /// <summary>
    /// 1、需要导入aliyun-net-sdk-core依赖
    /// 2、在阿里云配置签名+模板
    /// https://dysms.console.aliyun.com/dysms.htm?spm=5176.b60019767.ProductAndService--ali--widget-home-product-recent.dre0.525216d03rcBLN#/domestic/text/template
    /// </summary>
    public class SMSTool
    {
        /// <summary>
        /// 提供手机号---验证码
        /// 相关配置写死了
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static (bool, int, string) SendValidateCode(string phoneNumber, string code)
        {
            /*
                regionId:cn-hangzhou
                accessKeyId: ***REMOVED***
                accessKeySecret: ***REMOVED***
             */
            try
            {
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "***REMOVED***",
                    "***REMOVED***");

                DefaultAcsClient client = new DefaultAcsClient(profile);
                CommonRequest request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                // request.Protocol = ProtocolType.HTTP;
                request.AddQueryParameters("PhoneNumbers", phoneNumber); // 电话号码
                request.AddQueryParameters("SignName", "朝夕教育Eleven"); // 签名模板的名称
                request.AddQueryParameters("TemplateCode", "SMS_210070060"); // 验证码模板ID
                request.AddQueryParameters("TemplateParam", "{\"code\":\"" + code + "\"}"); // 验证码

                CommonResponse response = client.GetCommonResponse(request);
                Console.WriteLine(Encoding.Default.GetString(response.HttpResponse.Content));
                if (response.HttpStatus == 200)
                {
                    return new(true, response.HttpStatus, "短信已发送");
                }
                else
                {
                    return new(false, response.HttpStatus, "短信发送异常");
                }
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
                return new(false, 0, $"短信发送失败，服务器错误：{e.Message}");
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
                return new(false, 0, $"短信发送失败，客户端错误：{e.Message}");
            }
        }
    }
}