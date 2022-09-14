using System;

namespace GotifyClient.Entities
{
    public class GotifyMessageEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public int Id { get; set; }
        public int Appid { get; set; }
        public DateTime Date { get; set; }
    }
}