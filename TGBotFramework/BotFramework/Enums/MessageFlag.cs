using System;

namespace BotFramework.Enums
{
    [Flags]
    public enum MessageFlag
    {
        HasForward = 1 << 0,
        IsReply = 1 << 1,
        HasText = 1 << 2,
        HasEntity = 1 << 3,
        HasAudio = 1 << 4,
        HasDocument = 1 << 5,
        HasAnimation = 1 << 6,
        HasGame = 1 << 7,
        HasPhoto = 1 << 8,
        HasSticker = 1 << 9,
        HasVideo = 1 << 10,
        HasVoice = 1 << 11,
        HasVideoNote = 1 << 12,
        HasCaption = 1 << 13,
        HasContact = 1 << 14,
        HasLocation = 1 << 15,
        HasVenue = 1 << 16,
        HasPoll = 1 << 17,
        HasDice = 1 << 18,
        HasKeyboard = 1 << 19,

        All = HasForward |
              IsReply |
              HasText |
              HasEntity |
              HasAudio |
              HasDocument |
              HasAnimation |
              HasGame |
              HasPhoto |
              HasSticker |
              HasVideo |
              HasVoice |
              HasVideoNote |
              HasCaption |
              HasContact |
              HasLocation |
              HasVenue |
              HasPoll |
              HasDice |
              HasKeyboard
    }
}
