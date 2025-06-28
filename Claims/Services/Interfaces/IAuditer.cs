namespace Claims.Services.Interfaces
{
    public interface IAuditer
    {
        /// <summary>
        /// Audits a claim based on the provided claim identifier and HTTP request type.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to be audited.</param>
        /// <param name="httpRequestType">The HTTP request type associated with the audit operation, such as "GET" or "POST".</param>
        Task AuditClaim(string id, string httpRequestType);

        /// <summary>
        /// Audits a cover based on the provided cover identifier and HTTP request type.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to be audited.</param>
        /// <param name="httpRequestType">The HTTP request type associated with the audit operation, such as "GET" or "POST".</param>
        /// <returns></returns>
        Task AuditCover(string id, string httpRequestType);
    }
}