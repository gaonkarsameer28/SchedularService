using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuartzApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzApi.Controllers
{
    public class QRTZ_SchedularController : Controller
    {
        private readonly QRTZ_SchedularService _qrtz_schedularService;

        public QRTZ_SchedularController(QRTZ_SchedularService qrtz_schedularService)
        {
            _qrtz_schedularService = qrtz_schedularService;
        }


        [HttpGet]
        public ActionResult<List<QRTZ_Schedular>> Get() =>
            _qrtz_schedularService.Get();

        [HttpGet("{id:length(24)}", Name = "Getqrtz_schedular")]
        public ActionResult<QRTZ_Schedular> Get(string id)
        {
            var qrtz_schedular = (QRTZ_Schedular)_qrtz_schedularService.Get(id);

            if (qrtz_schedular == null)
            {
                return NotFound();
            }

            return qrtz_schedular;
        }


        [HttpGet]
        [Route("GetActiveJobs/{jobstatus}")]
        public ActionResult<List<QRTZ_Schedular>> GetActiveJobs(string jobstatus)
        {
            var qrtz_schedular = _qrtz_schedularService.GetActiveJobs(jobstatus);

            if (qrtz_schedular == null)
            {
                return NotFound();
            }

            return qrtz_schedular;
        }
        [HttpPost]
        [Route("GetJobIdByCron")]
        public ActionResult<string> GetJobIdByCron([FromBody] string cronexp)
        {
            //dynamic data = JsonConvert.DeserializeObject(cronexp);
            string jobid = _qrtz_schedularService.GetJobIdByCronAndURL(cronexp);

            if (jobid == null)
            {
                return NotFound();
            }

            return jobid;
        }

        [HttpPost]
        public ActionResult<QRTZ_Schedular> Create(QRTZ_Schedular qrtz_schedular)
        {
            _qrtz_schedularService.Create(qrtz_schedular);

            return CreatedAtRoute("Getqrtz_schedular", new { id = qrtz_schedular.Id.ToString() }, qrtz_schedular);
        }

        [HttpPost]
        [Route("InsertIntoQuartz")]
        public ActionResult<string> InsertIntoQuartz(QRTZ_Schedular qrtz_schedular)
        {
            string JobID = _qrtz_schedularService.InsertIntoQuartz(qrtz_schedular);
            return JobID;


        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, QRTZ_Schedular qrtz_schedularIn)
        {
            var qrtz_schedular = _qrtz_schedularService.Get(id);

            if (qrtz_schedular == null)
            {
                return NotFound();
            }

            _qrtz_schedularService.Update(id, qrtz_schedularIn);

            return NoContent();
        }
        //[HttpPut]
        //[Route("Update/{qrtz_schedularIn}")]
        //public IActionResult Update( List<QRTZ_Schedular> qrtz_schedularIn)
        //{
        //    _qrtz_schedularService.Update(qrtz_schedularIn);

        //    return NoContent();
        //}

        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody] List<QRTZ_Schedular> qrtz_schedularIn)
        {
            _qrtz_schedularService.Update(qrtz_schedularIn);

            return NoContent();
        }
        [HttpPost]
        [Route("UpdateExecuteDate")]
        public IActionResult UpdateExecuteDate([FromBody] string inputdata)
        {
            dynamic data = JsonConvert.DeserializeObject(inputdata);
            _qrtz_schedularService.UpdateExecuteDate(data.id.ToString(), DateTime.Parse(data.LastExcecutedate.ToString()));

            return NoContent();
        }
        [HttpPut("CompleteJobs")]
        //  [Route("CompleteJobs")]
        public IActionResult CompleteJobs([FromBody] string Id)
        {
            _qrtz_schedularService.CompleteJobs(Id);

            return NoContent();
        }

        [HttpPut]
        [Route("CompleteExpiredJobs")]
        public IActionResult CompleteExpiredJobs()
        {
            _qrtz_schedularService.CompleteExpiredJobs();

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var qrtz_schedular = _qrtz_schedularService.Get(id);

            if (qrtz_schedular == null)
            {
                return NotFound();
            }

            _qrtz_schedularService.Remove(qrtz_schedular.Id);

            return NoContent();
        }
    }
}
