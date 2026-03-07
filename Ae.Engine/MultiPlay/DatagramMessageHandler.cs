using Ae.MpCommsMessages.DatagramMessages.SpriteActions;
using NTDLS.DatagramMessaging;
using System;

namespace Ae.Engine.MultiPlay
{
    internal class DatagramMessageHandler(AeEngine engine)
        : IDmDatagramHandler
    {
        public void SiSpriteActionDelete(DmContext context, AeSpriteActionDelete payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionExplode(DmContext context, AeSpriteActionExplode payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionSpawn(DmContext context, AeSpriteActionSpawn payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionVector(DmContext context, AeSpriteActionMotion payload)
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
