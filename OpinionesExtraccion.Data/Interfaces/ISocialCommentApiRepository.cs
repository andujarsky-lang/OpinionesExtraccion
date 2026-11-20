using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpinionesExtraccion.Data.Models.Api;

namespace OpinionesExtraccion.Data.Interfaces;

public interface ISocialCommentApiRepository
{
    Task<IReadOnlyList<SocialCommentRecord>> GetSocialCommentsAsync(CancellationToken cancellationToken = default);
}