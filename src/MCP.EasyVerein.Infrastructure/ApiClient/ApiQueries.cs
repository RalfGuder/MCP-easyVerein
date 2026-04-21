namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Provides pre-built query strings for each easyVerein API resource endpoint.
/// </summary>
internal static class ApiQueries
{
    /// <summary>
    /// Shared <see cref="AnnouncementQuery"/> instance used to build announcement query strings with optional filters.
    /// </summary>
    internal static readonly AnnouncementQuery AnnouncementQuery = new();

    /// <summary>
    /// Gets the current announcement query string including field selection and any active filters.
    /// </summary>
    internal static string Announcement => AnnouncementQuery.ToString();

    /// <summary>
    /// Shared <see cref="BankAccountQuery"/> instance used to build bank-account query strings with optional filters.
    /// </summary>
    internal static readonly BankAccountQuery BankAccountQuery = new();

    /// <summary>
    /// Gets the current bank-account query string including field selection and any active filters.
    /// </summary>
    internal static string BankAccount => BankAccountQuery.ToString();

    /// <summary>
    /// Shared <see cref="BillingAccountQuery"/> instance used to build billing-account query strings with optional filters.
    /// </summary>
    internal static readonly BillingAccountQuery BillingAccountQuery = new();

    /// <summary>
    /// Gets the current billing-account query string including field selection and any active filters.
    /// </summary>
    internal static string BillingAccount => BillingAccountQuery.ToString();

    /// <summary>
    /// Shared <see cref="BookingProjectQuery"/> instance used to build booking-project query strings with optional filters.
    /// </summary>
    internal static readonly BookingProjectQuery BookingProjectQuery = new();

    /// <summary>
    /// Gets the current booking-project query string including field selection and any active filters.
    /// </summary>
    internal static string BookingProject => BookingProjectQuery.ToString();

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
    /// Shared <see cref="EventQuery"/> instance used to build event query strings with optional filters.
    /// </summary>
    internal static readonly EventQuery EventQuery = new();

    /// <summary>
    /// Gets the current event query string including field selection and any active filters.
    /// </summary>
    internal static string Event => EventQuery.ToString();
}
