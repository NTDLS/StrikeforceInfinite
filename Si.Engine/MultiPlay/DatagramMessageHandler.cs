using NTDLS.DatagramMessaging;
using Si.MpCommsMessages.DatagramMessages.SpriteActions;
using System;

namespace Si.Engine.MultiPlay
{
    internal class DatagramMessageHandler(EngineCore engineCore)
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

        public void SiSpriteActionVector(DmContext context, SiSpriteActionMotion payload)
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
