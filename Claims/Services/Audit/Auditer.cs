using Claims.Models.Auditing;
using Claims.Persistance;

namespace Claims.Services.Audit
{
    public class Auditer : IAuditer
    {
        private readonly AuditContext _auditContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Auditer"/> class.
        /// </summary>
        /// <param name="auditContext"></param>
        public Auditer(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        // </inheritdoc>
        public async Task AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await _auditContext.AddAsync(claimAudit);
            await _auditContext.SaveChangesAsync();
        }

        // </inheritdoc>
        public async Task AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await _auditContext.AddAsync(coverAudit);
            await _auditContext.SaveChangesAsync();
        }
    }
}
