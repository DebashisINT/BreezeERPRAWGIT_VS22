﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BreezeChat.ChatHelper
{
    public class MessageDetail
    {
        public int FromUserID { get; set; }
        public string FromUserName { get; set; }
        public int ToUserID { get; set; }
        public string ToUserName { get; set; }
        public string Message { get; set; }
        public DateTime chat_time { get; set; }
    }
}