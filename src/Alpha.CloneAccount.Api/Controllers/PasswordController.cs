using Alpha.CloneAccount.Api.Requests;
using System.Web.Http;
using Sunplus.Agent.Utilities;

namespace Alpha.CloneAccount.Api.Controllers
{
    [RoutePrefix("api/password")]
    public class PasswordController : ApiController
    {
        private readonly Encryptor encryptor;

        public PasswordController(Encryptor encryptor)
        {
            this.encryptor = encryptor;
        }

        [HttpPost]
        [Route("hash")]
        public IHttpActionResult Hash([FromBody] HashPasswordCommand command)
        {
            if (command.IsValid())
            {
                return Ok(encryptor.CfsEnCode(command.Password));
            }

            return BadRequest();
        }
    }
}