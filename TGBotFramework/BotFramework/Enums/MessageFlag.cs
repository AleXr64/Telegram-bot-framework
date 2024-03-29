﻿using System;

namespace BotFramework.Enums
{
    [Flags]
    public enum MessageFlag : long
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
        HasNewChatMembers = 1 << 20,
        HasLeftChatMember = 1 << 21,
        HasNewChatTitle = 1 << 22,
        HasNewChatPhoto = 1 << 23,
        HasDeleteChatPhoto = 1 << 24,
        HasGroupChatCreated = 1 << 25,
        HasSupergroupChatCreated = 1 << 26,
        HasPinnedMessage = 1 << 27,
        HasVideoChatScheduled = 1 << 28,
        HasVideoChatStarted = 1 << 29,
        HasVideoChatEnded = 1 << 30,
        HasVideoChatParticipantsInvited = 1 << 31,
        HasMediaSpoiler = (long)1 << 32,
        HasInvoice = (long)1 << 33,
        HasPassportData = (long)1 << 34,
        HasSuccessfulPayment = (long)1 << 35,


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
              HasKeyboard |
              HasLeftChatMember |
              HasNewChatMembers |
              HasNewChatTitle |
              HasNewChatPhoto |
              HasDeleteChatPhoto |
              HasGroupChatCreated |
              HasSupergroupChatCreated |
              HasPinnedMessage |
              HasVideoChatScheduled |
              HasVideoChatStarted |
              HasVideoChatEnded |
              HasVideoChatParticipantsInvited |
              HasMediaSpoiler | 
              HasInvoice |
              HasPassportData |
              HasSuccessfulPayment
    }
}
