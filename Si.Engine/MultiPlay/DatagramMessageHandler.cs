using NTDLS.DatagramMessaging;
using Si.MpComms.DatagramMessages.SpriteActions;
using System;

namespace Si.Engine.MultiPlay
{
    internal class DatagramMessageHandler(MultiPlayClient mpyClient)
        : IDmDatagramHandler
    {
        public void SiSpriteActionDelete(DmContext context, SiSpriteActionDelete payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionExplode(DmContext context, SiSpriteActionExplode payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionSpawn(DmContext context, SiSpriteActionSpawn payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionVector(DmContext context, SiSpriteActionVector payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}
