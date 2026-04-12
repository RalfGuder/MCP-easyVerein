namespace MCP.EasyVerein.Domain.ValueObjects
{
    /// <summary>Constants for easyVerein Event API field names used in JSON serialization.</summary>
    public static class EventFields
    {
        /// <summary>API field name for the unique event identifier.</summary>
        public const string Id = "id";
        /// <summary>API field name for the event name.</summary>
        public const string Name = "name";
        /// <summary>API field name for the event description.</summary>
        public const string Description = "description";
        /// <summary>API field name for the event prologue text.</summary>
        public const string Prologue = "prologue";
        /// <summary>API field name for the event note.</summary>
        public const string Note = "note";
        /// <summary>API field name for the event start date and time.</summary>
        public const string Start = "start";
        /// <summary>API field name for the event end date and time.</summary>
        public const string End = "end";
        /// <summary>API field name for whether the event spans the entire day.</summary>
        public const string AllDay = "allDay";
        /// <summary>API field name for the location name.</summary>
        public const string LocationName = "locationName";
        /// <summary>API field name for the location object reference.</summary>
        public const string LocationObject = "locationObject";
        /// <summary>API field name for the parent event reference.</summary>
        public const string Parent = "parent";
        /// <summary>API field name for the minimum number of participants.</summary>
        public const string MinParticipators = "minParticipators";
        /// <summary>API field name for the maximum number of participants.</summary>
        public const string MaxParticipators = "maxParticipators";
        /// <summary>API field name for the participation start date.</summary>
        public const string StartParticipation = "startParticipation";
        /// <summary>API field name for the participation end date.</summary>
        public const string EndParticipation = "endParticipation";
        /// <summary>API field name for the event access setting.</summary>
        public const string Access = "access";
        /// <summary>API field name for the weekdays the event occurs on.</summary>
        public const string Weekdays = "weekdays";
        /// <summary>API field name for the send mail check flag.</summary>
        public const string SendMailCheck = "sendMailCheck";
        /// <summary>API field name for whether the event is shown in the member area.</summary>
        public const string ShowMemberArea = "showMemberarea";
        /// <summary>API field name for whether the event is public.</summary>
        public const string IsPublic = "isPublic";
        /// <summary>API field name for mass participations.</summary>
        public const string MassParticipations = "massParticipations";
        /// <summary>API field name for whether the event is canceled.</summary>
        public const string Canceled = "canceled";
        /// <summary>API field name for whether the event is a reservation.</summary>
        public const string IsReservation = "isReservation";
        /// <summary>API field name for the event creator.</summary>
        public const string Creator = "creator";
        /// <summary>API field name for the reservation parent event reference.</summary>
        public const string ReservationParentEvent = "reservationParentEvent";

        /// <summary>API field name for the calendar reference.</summary>
        public const string Calendar = "calendar";

        /// <summary>API filter field name for start date greater than or equal.</summary>
        public const string StartGte = "start__gte";

        /// <summary>API filter field name for start date less than or equal.</summary>
        public const string StartLte = "start__lte";

        /// <summary>API filter field name for end date greater than or equal.</summary>
        public const string EndGte = "end__gte";

        /// <summary>API filter field name for end date less than or equal.</summary>
        public const string EndLte = "end__lte";

        /// <summary>API filter field name for filtering by multiple IDs.</summary>
        public const string IdIn = "id__in";

        /// <summary>API query parameter for result ordering.</summary>
        public const string Ordering = "ordering";

        /// <summary>API query parameter for full-text search.</summary>
        public const string Search = "search";
    }
}
