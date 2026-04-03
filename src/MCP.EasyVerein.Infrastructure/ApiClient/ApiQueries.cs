namespace MCP.EasyVerein.Infrastructure.ApiClient;

internal static class ApiQueries
{
    internal static readonly MemberQuery MemberQuery = new();
    internal static readonly ContactDetailsQuery ContactDetailsQuery = new();

    internal static string Member => MemberQuery.ToString();

    internal static string ContactDetails => ContactDetailsQuery.ToString();

    public const string Invoice =
        "query={id," + "invNumber,totalPrice,date,dateItHappend,dateSent,kind," +
        "description,receiver,relatedAddress," +
        "payedFromUser,approvedFromAdmin,canceledInvoice,bankAccount," +
        "gross,cancellationDescription,templateName,refNumber," +
        "isDraft,isTemplate,creationDateForRecurringInvoices," +
        "recurringInvoicesInterval,paymentInformation,isRequest," +
        "taxRate,taxName,actualCallStateName,callStateDelayDays," +
        "accnumber,guid,selectionAcc,removeFileOnDelete," +
        "isReceipt,mode,offerStatus," +
        "offerValidUntil,offerNumber,relatedOffer," +
        "closingDescription,useAddressBalance}";

    public const string Event =
        "query={id,name,description,prologue,note,start,end,allDay," +
        "locationName,locationObject,parent," +
        "minParticipators,maxParticipators," +
        "startParticipation,endParticipation,access,weekdays," +
        "sendMailCheck,showMemberarea,isPublic,massParticipations," +
        "canceled,isReservation,creator,reservationParentEvent}";
}
