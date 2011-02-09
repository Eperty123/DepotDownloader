﻿/*
 * This file is subject to the terms and conditions defined in
 * file 'license.txt', which is part of this source code package.
 */



using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace SteamKit2
{
    public class DSClient : ServerClient
    {
        public IPEndPoint[] GetServerList( EServerType type )
        {
            return this.GetServerList( type, null );
        }

        public IPEndPoint[] GetServerList( EServerType type, string userName )
        {
            List<IPEndPoint> serverList = new List<IPEndPoint>();

            try
            {

                if ( !this.HandshakeServer( EServerType.GeneralDirectoryServer ) )
                {
                    DebugLog.WriteLine( "DSClient", "GetServerList failed handshake." );

                    Socket.Disconnect();
                    return null;
                }

                bool bRet = false;

                if ( userName != null )
                {
                    byte[] userHash = CryptoHelper.JenkinsHash( Encoding.ASCII.GetBytes( userName ) );
                    uint userData = BitConverter.ToUInt32( userHash, 0 ) & 1;

                    bRet = this.SendCommand( ( byte )type, NetHelpers.EndianSwap( userData ) );
                }
                else
                {
                    bRet = this.SendCommand( ( byte )type );
                }

                if ( !bRet )
                {
                    DebugLog.WriteLine( "DSClient", "GetServerList failed sending EServerType command." );

                    Socket.Disconnect();
                    return null;
                }

                TcpPacket packet = Socket.ReceivePacket();
                DataStream ds = new DataStream( packet.GetPayload(), true );

                ushort numAddrs = ds.ReadUInt16();

                for ( int x = 0 ; x < numAddrs ; ++x )
                {
                    IPAddrPort ipAddr = IPAddrPort.Deserialize( ds.ReadBytes( 6 ) );

                    serverList.Add( ipAddr );
                }
            }
            catch ( Exception ex )
            {
                DebugLog.WriteLine( "DSClient", "GetServerList threw an exception.\n{0}", ex.ToString() );

                Socket.Disconnect();
                return null;
            }

            return serverList.ToArray();
        }

        /*
        public IPEndPoint[] GetServerList( EServerType type )
        {
            return GetServerList( type, null );
        }

        public IPEndPoint[] GetServerList( EServerType type, string userName )
        {
            List<IPEndPoint> serverList = new List<IPEndPoint>();

            try
            {

                if ( !this.HandshakeServer( EServerType.GeneralDirectoryServer ) )
                {
                    DebugLog.WriteLine( "DSClient", "GetServerList failed handshake." );

                    Socket.Disconnect();
                    return null;
                }

                bool bRet = false;

                if ( userName != null )
                {
                    byte[] userHash = CryptoHelper.JenkinsHash( Encoding.ASCII.GetBytes( userName ) );

                    bRet = this.SendCommand( ( byte )type, ( uint )0 );
                }
                else
                {
                    bRet = this.SendCommand( ( byte )type, (uint)0 );
                }

                if ( !bRet )
                {
                    DebugLog.WriteLine( "DSClient", "GetServerList failed sending EServerType command." );

                    Socket.Disconnect();
                    return null;
                }

                TcpPacket packet = Socket.ReceivePacket();
                DataStream ds = new DataStream( packet.GetPayload(), true );

                ushort numAddrs = ds.ReadUInt16();

                for ( int x = 0 ; x < numAddrs ; ++x )
                {
                    IPAddrPort ipAddr = IPAddrPort.Deserialize( ds.ReadBytes( 6 ) );

                    serverList.Add( ipAddr );
                }
            }
            catch ( Exception ex )
            {
                DebugLog.WriteLine( "DSClient", "GetServerList threw an exception.\n{0}", ex.ToString() );

                Socket.Disconnect();
                return null;
            }

            return serverList.ToArray();
        }*/
    }
}
