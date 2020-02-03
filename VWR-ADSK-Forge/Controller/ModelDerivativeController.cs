using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Autodesk.Forge;
using Autodesk.Forge.Model;

namespace Backend.Controller
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ModelDerivativeController : ControllerBase
    {
        #region Methods
        /// <summary>
        /// Start the translation job for a give bucketKey/objectName
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/forge/modelderivative/jobs")]
        public async Task<dynamic> TranslateObject([FromBody]Model.TranslatedModel objModel)
        {
            //dynamic oauth = await OAuthController.GetInternalAsync();

            // prepare the payload
            List<JobPayloadItem> outputs = new List<JobPayloadItem>()
            {
            new JobPayloadItem(
                JobPayloadItem.TypeEnum.Svf,
                new List<JobPayloadItem.ViewsEnum>()
                {
                    JobPayloadItem.ViewsEnum._2d,
                    JobPayloadItem.ViewsEnum._3d
                })
            };
            JobPayload job;
            job = new JobPayload(new JobPayloadInput(objModel.objectName), new JobPayloadOutput(outputs));

            // start the translation
            DerivativesApi derivative = new DerivativesApi();
            //derivative.Configuration.AccessToken = oauth.access_token;
            dynamic jobPosted = await derivative.TranslateAsync(job);
            return jobPosted;
        }

        #endregion
    }
}