using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net.NetworkInformation;
using TamaApi.clsMod;
using TamaApi.Services.db;
using TamaApi.Services.Main;
using static TamaApi.Services.db.DbService;

namespace TamaApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger<MainController> _logger;
        private readonly IMainService _mainService;

        public MainController(IConfiguration config, ILogger<MainController> logger, IMainService mainService)
        {
            _logger = logger;
            Configuration = config;
            _mainService = mainService;
        }

        [HttpGet("Ping")]
        public IActionResult Ping(string? test = "")
        {
            //_logger.LogInformation($"XXX_MainController (Ping)");
            return Ok($"Pinging == Ok\n{test}");
        }

        [HttpPost("GetSetupTable")]
        public IActionResult GetSetupTable()
        {
            try
            {
                _logger.LogInformation($"XXX_GetSetupTable");
                dynamic msgRes = _mainService.GetSetupTable();
                //_logger.LogInformation($"XXX_GetSetupTable ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_GetSetupTable ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }


        [HttpPost("GetTable"), Authorize]
        //[HttpPost("GetTable")]
        public IActionResult GetTable(GetTableCP getTableCP)
        {
            try
            {
                _logger.LogInformation($"XXX_GetSetupTable");
                dynamic msgRes = _mainService.GetTable(getTableCP);
                //_logger.LogInformation($"XXX_GetSetupTable ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_GetSetupTable ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("PingClinicId")]
        //[Obsolete]
#else
        [HttpPost("PingClinicId"), Authorize]
        [Obsolete]
        //[Obsolete]
#endif
        public IActionResult PingClinicId(int ClinicId)
        {
            try
            {
                _logger.LogInformation($"XXX_Main_PingIp ===> ClinicId = {ClinicId}");
                Ping p = new();
                PingReply r;

                dynamic res;
                if (ClinicId == 0)
                {
                    res = new
                    {
                        Speed = 10000,
                        IpStatus = 1
                    };
                    return Ok(res);
                }

                DataTable tbl = DbService.GetTable(getTableCP: new GetTableCP() { TableName = "Clinic", IntFN = "ID", IntFV = ClinicId });
                if (tbl.Rows.Count == 0)
                {
                    res = new
                    {
                        Speed = 10000,
                        IpStatus = 1
                    };
                    return Ok(res);
                }

                if (DBNull.Value == tbl.Rows[0]["IpAddress"])
                {
                    res = new
                    {
                        Speed = 10000,
                        IpStatus = 1
                    };
                    return Ok(res);
                }

                string Ip = (String)tbl.Rows[0]["IpAddress"];
                r = p.Send(Ip);

                long s1 = 0;
                long s2 = 0;
                long s3 = 0;
                long s4 = 0;
                long s5 = 0;
                long s6 = 0;
                long s7 = 0;
                long s8 = 0;

                if (r.Status == IPStatus.Success)
                {
                    s1 = r.RoundtripTime;
                    s2 = p.Send(Ip).RoundtripTime;
                    s3 = p.Send(Ip).RoundtripTime;
                    s4 = p.Send(Ip).RoundtripTime;
                    s5 = p.Send(Ip).RoundtripTime;
                    s6 = p.Send(Ip).RoundtripTime;
                    s7 = p.Send(Ip).RoundtripTime;
                    s8 = p.Send(Ip).RoundtripTime;
                }

                res = new
                {
                    Speed = (s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8) / 8,
                    IpStatus = r.Status
                };

                _logger.LogInformation($"XXX_Main_PingIp (Ping)===>{JsonConvert.SerializeObject(res)}");

                return Ok(res);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_Main_PingIp ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }


#if DEBUG
        [HttpPost("ExecStoredProcedure")]
#else
        [HttpPost("ExecStoredProcedure"), Authorize]
#endif
        public IActionResult ExecStoredProcedure(TamaRequest tamaRequest)
        {
            try
            {
                _logger.LogInformation($"XXX_ExecStoredProcedure ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");
                dynamic msgRes = _mainService.ExecStoredProcedure(tamaRequest);
                //_logger.LogInformation($"XXX_ExecStoredProcedure ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }



#if DEBUG
        [HttpPost("ExecCmd")]
#else
        [HttpPost("ExecCmd"), Authorize]
#endif
        public IActionResult ExecCmd(TamaRequestCmd tamaRequestCmd)
        {
            try
            {
                _logger.LogInformation($"XXX_ExecCmd===> tamaRequestCmd = {JsonConvert.SerializeObject(tamaRequestCmd)}");
                dynamic msgRes = _mainService.ExecCmd(tamaRequestCmd);
                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("DoSomeThings")]
#else
        [HttpPost("DoSomeThings"), Authorize]
#endif
        public IActionResult DoSomeThings(TamaRequest tamaRequest)
        {
            try
            {
                _logger.LogInformation($"XXX_DoSomeThings ===> tamaRequest = {JsonConvert.SerializeObject(tamaRequest)}");
                dynamic msgRes = _mainService.DoSomeThings(tamaRequest);
                //_logger.LogInformation($"XXX_ExecStoredProcedure ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("WhatsAppMsgAnalysisRes")]
#else
        [HttpPost("WhatsAppMsgAnalysisRes"), Authorize]
#endif
        public IActionResult WhatsAppMsgAnalysisRes(ClsWhatsAppMsgAnalysisRes clsWhatsAppMsgAnalysisRes)
        {
            try
            {
                _logger.LogInformation($"WhatsAppMsgAnalysisRes ===> ClsWhatsAppMsgAnalysisRes = {JsonConvert.SerializeObject(clsWhatsAppMsgAnalysisRes)}");
                dynamic msgRes = _mainService.WhatsAppMsgAnalysisRes(
                    clsWhatsAppMsgAnalysisRes.ClinicNo,
                    clsWhatsAppMsgAnalysisRes.FileName,
                    clsWhatsAppMsgAnalysisRes.OtherInfo,
                    clsWhatsAppMsgAnalysisRes.customerPhoneNo,
                    _logger);
                //_logger.LogInformation($"WhatsAppMsgAnalysisRes ===> msgRes = {JsonConvert.SerializeObject(msgRes.Result)}");
                return StatusCode(StatusCodes.Status200OK, msgRes.Result);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_WhatsAppMsgAnalysisRes ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("SmsLinkAnalysisRes")]
#else
        [HttpPost("SmsLinkAnalysisRes"), Authorize]
#endif
        public IActionResult SmsLinkAnalysisRes(ClsSmsLinkAnalysisRes clsSmsLinkAnalysisRes)
        {
            try
            {
                _logger.LogInformation($"SmsLinkAnalysisRes ===> ClsSmsLinkAnalysisRes = {JsonConvert.SerializeObject(clsSmsLinkAnalysisRes)}");
                dynamic msgRes = _mainService.SmsLinkAnalysisRes(clsSmsLinkAnalysisRes, _logger);
                //_logger.LogInformation($"WhatsAppMsgAnalysisRes ===> msgRes = {JsonConvert.SerializeObject(msgRes.Result)}");
                return StatusCode(StatusCodes.Status200OK, msgRes.Result);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_WhatsAppMsgAnalysisRes ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("WhatsAppMsg")]
#else
    [HttpPost("WhatsAppMsg"), Authorize]
#endif
        public IActionResult WhatsAppMsg(ClsWhatsAppMsg clsWhatsAppMsg)
        {
            try
            {
                _logger.LogInformation($"WhatsAppMsg ===> ClsWhatsAppMsg = {JsonConvert.SerializeObject(clsWhatsAppMsg)}");
                dynamic msgRes = _mainService.WhatsAppMsg(clsWhatsAppMsg, _logger);
                //_logger.LogInformation($"WhatsAppMsg ===> msgRes = {JsonConvert.SerializeObject(msgRes.Result)}");
                return StatusCode(StatusCodes.Status200OK, msgRes.Result);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_WhatsAppMsg===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("GetUrlPay")]
#else
    [HttpPost("GetUrlPay"), Authorize]
#endif
        public async Task<IActionResult> GetUrlPay(ClsUrlPay clsUrlPay)
        {
            try
            {
                _logger.LogInformation($"GetUrlPay ===> ClsGetUrlPay = {JsonConvert.SerializeObject(clsUrlPay)}");
                dynamic msgRes = await _mainService.GetUrlPay(clsUrlPay, _logger);
                _logger.LogInformation($"GetUrlPay ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_GetUrlPay===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("Notification")]
#else
    [HttpPost("Notification"), Authorize]
#endif
        public async Task<IActionResult> Notification(ApiNotification apiNotification)
        {
            try
            {
                _logger.LogInformation($"Notification ===> Message = {JsonConvert.SerializeObject(apiNotification)}");
                dynamic msgRes = await _mainService.Notification(apiNotification, _logger);
                //_logger.LogInformation($"Notification ===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_Notification===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("GetItemsForCompanyCo")]
#else
        [HttpPost("GetItemsForCompanyCo"), Authorize]
#endif
        public IActionResult GetItemsForCompanyCo(int clinicId, int idFrom, int idTo)
        {
            try
            {
                _logger.LogInformation($"XXX_GetItemsForCompanyCo===> clinicId = {clinicId}");

                if (!User.Identity!.IsAuthenticated) return Unauthorized("المستخدم غير مصرح له بالوصول.");

                string phoneNumber = User.Claims.FirstOrDefault(c => c.Type.EndsWith("name"))?.Value.ToString()!;
                if (phoneNumber.Length != 13) return Unauthorized("لم نجد رقم الهاتف في التوكن");

                int RemoteContractCoId = _mainService.GetRemoteContractCoId(phoneNumber, clinicId);
                if (RemoteContractCoId == 0) return Unauthorized("لا يمكن لهذا المستخدم القراءة من هذه المصحة");

                dynamic msgRes = _mainService.DoSomeThings(new()
                {
                    spName = "GetItemsForCompanyCo",
                    clinicId = clinicId,
                    paras = [
                        new() { Name = "RemoteContractCoId", Value = RemoteContractCoId.ToString(), DataType = SqlDbType.Int },
                        new() { Name = "idFrom", Value = idFrom.ToString(), DataType = SqlDbType.Int },
                        new() { Name = "idTo", Value = idTo.ToString(), DataType = SqlDbType.Int },
                    ]
                });

                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

#if DEBUG
        [HttpPost("GetVoucherCompanyCo")]
#else
        [HttpPost("GetVoucherCompanyCo"), Authorize]
#endif
        public IActionResult GetVoucherCompanyCo(ClsVoucherCompanyCo clsVoucherCompanyCo)
        {
            try
            {
                _logger.LogInformation($"XXX_GetVoucherCompanyCo===> clinicId = {clsVoucherCompanyCo.ClinicId}");

                if (!User.Identity!.IsAuthenticated) return Unauthorized("المستخدم غير مصرح له بالوصول.");

                string phoneNumber = User.Claims.FirstOrDefault(c => c.Type.EndsWith("name"))?.Value.ToString()!;
                if (phoneNumber.Length != 13) return Unauthorized("لم نجد رقم الهاتف في التوكن");

                int RemoteContractCoId = _mainService.GetRemoteContractCoId(phoneNumber, clsVoucherCompanyCo.ClinicId);
                if (RemoteContractCoId == 0) return Unauthorized("لا يمكن لهذا المستخدم القراءة من هذه المصحة");

                dynamic msgRes = _mainService.DoSomeThings(new()
                {
                    spName = "GetVoucherCompanyCo",
                    clinicId = clsVoucherCompanyCo.ClinicId,
                    paras = [
                        new() { Name = "RemoteContractCoId", Value = RemoteContractCoId.ToString(), DataType = SqlDbType.Int },
                        new() { Name = "DateFrom", Value = clsVoucherCompanyCo.DateFrom.ToString(), DataType = SqlDbType.DateTime },
                        new() { Name = "DateTo", Value = clsVoucherCompanyCo.DateTo.ToString(), DataType = SqlDbType.DateTime },
                    ]
                });

                //_logger.LogInformation($"XXX_ExecCmd===> msgRes = {JsonConvert.SerializeObject(msgRes)}");
                return StatusCode(StatusCodes.Status200OK, msgRes);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"XXX_ExecStoredProcedure ===> Error = {Ex.Message}");
                return Error(Ex);
            }
        }

        private IActionResult Error(Exception Ex)
        {
            if (Ex.Source == "TamaApi")
                return StatusCode(StatusCodes.Status423Locked, Ex.Message);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, Ex.Message);
        }

    }
}
