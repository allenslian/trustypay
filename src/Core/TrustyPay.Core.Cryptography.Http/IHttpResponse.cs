using System.Collections.Generic;

namespace TrustyPay.Core.Cryptography.Http
{
    /// <summary>
    /// A http response interface
    /// </summary>
    /// <typeparam name="U">A response class</typeparam>
    public interface IHttpResponse<U> where U : class
    {
        /// <summary>
        /// Parse http response body
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        U ParseBody(string content);
    }
}