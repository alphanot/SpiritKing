using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SpiritKing.Controllers
{
    public class MusicController
    {
        public static Song OrganTheme { get; private set; }
        public static Song Ambience { get; private set; }

        public MusicController(ContentManager Content)
        {
            Ambience = Content.Load<Song>("Audio/dungeon_ambient_1");
            OrganTheme = Content.Load<Song>("Audio/Organ Theme");
        }

        public static void PlaySong(Song song, bool repeat = false)
        {
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = repeat;
        }

        public void Unload()
        {
            OrganTheme.Dispose();
        }
    }
}
