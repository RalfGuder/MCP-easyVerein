namespace MCP.EasyVerein.Infrastructure.ApiClient;

internal static class ApiQueries
{
    public const string Member =
        "{id,emailOrUserName,membershipNumber,joinDate,resignationDate," +
        "resignationNoticeDate,paymentAmount,paymentIntervallMonths," +
        "_paymentStartDate,_isApplication,_applicationDate," +
        "_applicationWasAcceptedAt,_isChairman,_chairmanPermissionGroup," +
        "_isBlocked,blockReason,_profilePicture,_relatedMember," +
        "_editableByRelatedMembers,useBalanceForMembershipFee," +
        "bulletinBoardNewPostNotification,declarationOfApplication," +
        "sepaMandateFile,requirePasswordChange,_isMatrixSearchable," +
        "matrixBlockReason,blockedFromMatrix,_matrixCommunicationPermission," +
        "useMatrixGroupSettings,showWarningsAndNotesToAdminsInProfile," +
        "signatureText," +
        "contactDetails{id,firstName,familyName,salutation,nameAffix," +
        "dateOfBirth,privateEmail,companyEmail,primaryEmail," +
        "_preferredEmailField,preferredCommunicationWay," +
        "privatePhone,companyPhone,mobilePhone," +
        "street,addressSuffix,city,state,zip,country," +
        "_isCompany,companyName,companyStreet,companyCity,companyState," +
        "companyZip,companyCountry,companyAddressSuffix," +
        "companyNameInvoice,companyStreetInvoice,companyCityInvoice," +
        "companyStateInvoice,companyZipInvoice,companyCountryInvoice," +
        "companyAddressSuffixInvoice,companyPhoneInvoice,companyEmailInvoice," +
        "professionalRole,balance,iban,bic,bankAccountOwner," +
        "sepaMandate,sepaDate,methodOfPayment,datevAccountNumber," +
        "internalNote,invoiceCompany,sendInvoiceCompanyMail," +
        "addressCompany,customPaymentMethod}}";

    public const string ContactDetails =
        "{id,firstName,familyName,salutation,nameAffix,dateOfBirth," +
        "privateEmail,companyEmail,primaryEmail,_preferredEmailField," +
        "preferredCommunicationWay,privatePhone,companyPhone,mobilePhone," +
        "street,addressSuffix,city,state,zip,country," +
        "_isCompany,companyName,companyStreet,companyCity,companyState," +
        "companyZip,companyCountry,companyAddressSuffix," +
        "companyNameInvoice,companyStreetInvoice,companyCityInvoice," +
        "companyStateInvoice,companyZipInvoice,companyCountryInvoice," +
        "companyAddressSuffixInvoice,companyPhoneInvoice,companyEmailInvoice," +
        "professionalRole,balance,iban,bic,bankAccountOwner," +
        "sepaMandate,sepaDate,methodOfPayment,datevAccountNumber," +
        "internalNote,invoiceCompany,sendInvoiceCompanyMail," +
        "addressCompany,customPaymentMethod}";

    public const string Invoice =
        "{id,invNumber,totalPrice,date,dateItHappend,dateSent,kind," +
        "description,receiver,relatedAddress,relatedBookings," +
        "payedFromUser,approvedFromAdmin,canceledInvoice,bankAccount," +
        "gross,cancellationDescription,templateName,refNumber," +
        "isDraft,isTemplate,creationDateForRecurringInvoices," +
        "recurringInvoicesInterval,paymentInformation,isRequest," +
        "taxRate,taxName,actualCallStateName,callStateDelayDays," +
        "accnumber,guid,selectionAcc,removeFileOnDelete," +
        "customPaymentMethod,isReceipt,mode,offerStatus," +
        "offerValidUntil,offerNumber,relatedOffer," +
        "closingDescription,useAddressBalance}";

    public const string Event =
        "{id,name,description,prologue,note,start,end,allDay," +
        "locationName,locationObject,parent," +
        "minParticipators,maxParticipators," +
        "startParticipation,endParticipation,access,weekdays," +
        "sendMailCheck,showMemberarea,isPublic,massParticipations," +
        "canceled,isReservation,creator,reservationParentEvent}";
}
