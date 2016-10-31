using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundCollector.Components.Base
{
    class BaseGameComponent
    {
        protected readonly MainGame _mainGame;

        public BaseGameComponent(MainGame mainGame)
        {
            this._mainGame = mainGame;
        }
    }
}
