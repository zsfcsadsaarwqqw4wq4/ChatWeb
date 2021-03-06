﻿using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class JwtHelper
    {
        const string secret = "RHKJ";

        public static string CreateToken(User us,DateTime time)
        {
            try
            {
                AuthInfo info = new AuthInfo { LoginID = us.LoginID, ID = us.ID,Iat=time};
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                return encoder.Encode(info, secret);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static AuthInfo GetJwtDecode(string token)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
            var userInfo = decoder.DecodeToObject<AuthInfo>(token, secret, verify: true);
            return userInfo;
        }
    }

    public class AuthInfo
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginID { get; set; }
        /// <summary>
        /// 用户主键id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// jwt的签发时间
        /// </summary>
        public DateTime Iat { get; set; }
    }
}
