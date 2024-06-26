﻿namespace Server.Items
{
    public class CoffinPiece : BaseItem
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1116783;

        [Constructable]
        public CoffinPiece() : base(Utility.RandomList(7481, 7480, 7479, 7452, 7451, 7450))
        {
        }

        public CoffinPiece(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}