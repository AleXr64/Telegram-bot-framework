using BotFramework.Enums;
namespace BotFramework.Attributes
{
    public class UpdateAttribute:HandlerAttribute
    {
        internal UpdateFlag UpdateFlags;

        public UpdateAttribute()
        {
            UpdateFlags = UpdateFlag.All;
        }

        public UpdateAttribute(UpdateFlag updateFlags)
        {
            UpdateFlags = updateFlags;
        }
        protected override bool CanHandle(HandlerParams param)
        {
            return UpdateFlags.HasFlag(UpdateFlag.All) || UpdateFlags.HasFlag((UpdateFlag)(1UL << (int)param.Type));
            
        }

    }
}
