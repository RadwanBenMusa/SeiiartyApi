using FirebaseAdmin.Messaging;
using Serilog.Parsing;

namespace TamaApi.clsMod
{
    public class GeneralModles
    {
    }
    public class GetTableCP
    {

        public required string TableName { get; set; }
        public string? IntFN { get; set; } = "";
        public int? IntFV { get; set; } = 0;
        public string? StrFN { get; set; } = "";
        public string? StrFV { get; set; } = "";
        public bool? DeletionDateIsNull { get; set; } = null;
        public string? OtherT1 { get; set; } = "";
        public string? OtherT1F1 { get; set; } = "";
    }
    public class AvailableAppointments
    {
        public DateTime appointmentsDate{ get; set; }
        public int OpdScheduleId { get; set; }
        public DateTime startTime { get; set; }
        public bool deleted { get; set; }
    }
    public enum EnumNotificationType
    {
        contactUs,
        searchMedecine,
        clinicAdmins,
        examWaitinglist,
        analysisResPatient,
        analysisResDoc
    }
    public class ApiNotification
    {
        public Notification? Notification { get; set; }
        public Dictionary<string, string>? Data { get; set; }
        public string? Token { get; set; }
        public EnumNotificationType NotificationType { get; set; }
    }
    public enum EnumPaymentGateType
    {
        walletRecharge,
        patientAppointment,
        orderPayment,
        activeClinic,
        subscribeMachine,
        subscribeWhatsApp,
        subscribeSms,
    }
    public class ClsUrlPay
    {
        public double Amount { get; set; }
        public int ClinicId { get; set; }
        public int CreationUserId { get; set; }
        public bool RealEnvironment { get; set; }
        public required string UserPhoneNo { get; set; }
        public EnumPaymentGateType PaymentGateType { get; set; }
        public ClsAppointment? Appointment { get; set; }
        public List<ClsActiveClinic>? ActiveClinicList { get; set; }
        public ClsMachine? Machine { get; set; }
        public List<int>? OrderDetailsIds { get; set; }
        public bool Moamalat { get; set; }
    }

    public class ClsActiveClinic
    {
        public int YearNo { get; set; }
        public int MonthNo { get; set; }
       
    }
    public class ClsMachine
    {
        public int MachineId { get; set; }
        public DateTime ExDate { get; set; }
        public bool Com { get; set; }

    }

    public class ClsAppointment 
    {
        public int PatientId { get; set; }
        public int RemoteOpdScheduleId { get; set; }
        public required string AppointmentDate { get; set; }
        public required string DocName { get; set; }
    }
    public class ClsVoucherCompanyCo {
        public int ClinicId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    //public class MsgRes
    //{
    //    public int? ID { set; get; }
    //    public int MsgId { set; get; }
    //    //1 العملية صحيحة
    //    //-1 خطاء غير متوقع
    //    //X خطا متوقع
    //    public string? MsgAr { set; get; }
    //    public string? MsgEn { set; get; }
    //    public dynamic? Data { set; get; }
    //}

}

