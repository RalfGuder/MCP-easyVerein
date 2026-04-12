namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Provides pre-built query strings for each easyVerein API resource endpoint.
/// </summary>
internal static class ApiQueries
{
    /// <summary>
    /// Shared <see cref="MemberQuery"/> instance used to build member query strings with optional filters.
    /// </summary>
    internal static readonly MemberQuery MemberQuery = new();

    /// <summary>
    /// Shared <see cref="ContactDetailsQuery"/> instance used to build contact details query strings with optional filters.
    /// </summary>
    internal static readonly ContactDetailsQuery ContactDetailsQuery = new();

    /// <summary>
    /// Gets the current member query string including field selection and any active filters.
    /// </summary>
    internal static string Member => MemberQuery.ToString();

    /// <summary>
    /// Gets the current contact details query string including field selection and any active filters.
    /// </summary>
    internal static string ContactDetails => ContactDetailsQuery.ToString();

    /// <summary>
    /// Shared <see cref="BookingQuery"/> instance used to build booking query strings with optional filters.
    /// </summary>
    internal static readonly BookingQuery BookingQuery = new();

    /// <summary>
    /// Gets the current booking query string including field selection and any active filters.
    /// </summary>
    internal static string Booking => BookingQuery.ToString();

    /// <summary>
    /// Shared <see cref="CalendarQuery"/> instance used to build calendar query strings with optional filters.
    /// </summary>
    internal static readonly CalendarQuery CalendarQuery = new();

    /// <summary>
    /// Gets the current calendar query string including field selection and any active filters.
    /// </summary>
    internal static string Calendar => CalendarQuery.ToString();

    /// <summary>
    /// Query string for invoice endpoints specifying all requested fields.
    /// </summary>
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

    /// <summary>
    /// Query string for event endpoints specifying all requested fields.
    /// </summary>
    public const string Event =
        "query={id,name,description,prologue,note,start,end,allDay," +
        "locationName,locationObject,parent," +
        "minParticipators,maxParticipators," +
        "startParticipation,endParticipation,access,weekdays," +
        "sendMailCheck,showMemberarea,isPublic,massParticipations," +
        "canceled,isReservation,creator,reservationParentEvent}";
}
