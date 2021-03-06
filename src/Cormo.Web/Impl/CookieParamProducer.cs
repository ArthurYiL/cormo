﻿using System.ComponentModel;
using System.Linq;
using System.Web;
using Cormo.Injects;
using Cormo.Injects.Exceptions;
using Cormo.Web.Api;

namespace Cormo.Web.Impl
{
    public class CookieParamProducer
    {
        [Produces]
        [CookieParam]
        protected T GetCookieParam<T>(IInjectionPoint ip)
        {
            if(ip == null)
                throw new InjectionException("CookieParam needs injection point");

            var context = HttpContext.Current;
            var converter = TypeDescriptor.GetConverter(typeof (T));
            if(context == null || converter == null)
                throw new UnsatisfiedDependencyException(ip);

            var httpCookie = context.Request.Cookies.Get(GetCookieName(ip));
            if (httpCookie == null)
                return GetDefaultValue<T>(ip);

            var cookieValue = httpCookie.Value;
            return (T) converter.ConvertFromString(cookieValue);
        }

        protected string GetCookieName(IInjectionPoint ip)
        {
            var attrName = ip.Qualifiers.OfType<CookieParamAttribute>().Select(x => x.Name).SingleOrDefault();
            if (string.IsNullOrEmpty(attrName))
            {
                var methodParam = ip as IMethodParameterInjectionPoint;
                if (methodParam != null)
                    return methodParam.ParameterInfo.Name;

                return ip.Member.Name;
            }
            return attrName;
        }

        protected T GetDefaultValue<T>(IInjectionPoint ip)
        {
            var attrDefault = ip.Qualifiers.OfType<CookieParamAttribute>().Select(x => x.Default).OfType<T>().ToArray();
            if (attrDefault.Any())
                return attrDefault[0];

            return default(T);
        }
    }
}