﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipBaseSmall : GenericShipBase
    {

        public ShipBaseSmall(GenericShip host) : base(host)
        {
            Size = BaseSize.Small;
            PrefabPath = "Prefabs/ShipModel/ShipBase/ShipBaseSmall";
            TemporaryPrefabPath = "Prefabs/ShipModel/ShipBase/TemporaryShipBaseSmall";

            HALF_OF_SHIPSTAND_SIZE = 0.5f;
            SHIPSTAND_SIZE = 1f;
            HALF_OF_FIRINGARC_SIZE = 0.425f;

            CreateShipBase();
        }

        protected override void CreateShipBase()
        {
            base.CreateShipBase();

            Host.GetShipAllPartsTransform().localPosition = Host.GetShipAllPartsTransform().localPosition + new Vector3(0f, 0f, -0.5f);
        }

    }
}
