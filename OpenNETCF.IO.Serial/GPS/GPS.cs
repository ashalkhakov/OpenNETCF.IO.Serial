//==========================================================================================
//
//		OpenNETCF.IO.Serial.GPS
//		Copyright (c) 2003-2009, OpenNETCF Consulting, LLC
//      http://www.opennetcf.com
//
//      Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
//      associated documentation files (the "Software"), to deal in the Software without restriction, 
//      including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//      sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
//      furnished to do so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all copies or 
//      substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
//      BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
//      DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//
//==========================================================================================
using System;
using OpenNETCF.IO.Serial;
using System.Threading;
using System.Text;
using System.Collections;
using System.IO;
using System.Globalization;
using Microsoft.Win32;

namespace OpenNETCF.IO.Serial.GPS
{

	public class GPS
	{
		#region private members
		Port cp=null;
		string comport="COM1:";
		BaudRates baudrate=BaudRates.CBR_4800;
		BaudRates minbaudrate = BaudRates.CBR_4800;
		BaudRates maxbaudrate = BaudRates.CBR_19200;

		string strreceived="";
		int sentencecount=0;

		bool InitDistance = false;

		private States state=States.Stopped;
		private bool autodiscovery=false;

		private Thread thrd=null;

		private bool traceon=false;
		private string tracefile=@"\gpslog.txt";

		private bool demomodeon=false;
		private string demofile=@"gpsdemo.txt";
		string lasterror="";
		
		private Position waypoint=null;
		decimal distancetowaypoint=0;
		decimal bearingtowaypoint=0;
		decimal coursecorrection=0;

		// Position dilution of precision
		// The best value is near to 0
		decimal pdop=50; // The max if it's not present

		// Horizontal dilution of precision
		// The best value is near to 0
		decimal hdop=50; // The max if it's not present

		// Vertical dilution of precision
		// The best value is near to 0
		decimal vdop=50; // The max if it's not present
		
		// Maximum Allowable HDOP Error
		// The best value is near to 0
		decimal hdopmaxerror=6;

		//number of satellites in view
		int nbsatinview=0;

		//number of satellites used
		int nbsatused=0;

		// Fix mode Auto,Manual
		Fix_Mode fixmode=OpenNETCF.IO.Serial.GPS.Fix_Mode.Auto;
		// Fix indicator NotSet,_2D,_3D
		Fix_Indicator fixindicator=OpenNETCF.IO.Serial.GPS.Fix_Indicator.NotSet;
		// Fix type NotSet,	NoAltitude,	WithAltitude
		Fix_Type fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NotSet;
		// Fix typemode NotSet, SPS, DSPS, PPS, RTK
		Fix_TypeMode fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.NotSet;
		
    #endregion

		#region public members
		public void DebugIt()
		{

			reset_gps_vars();

			string gpsdata="";
		
			try
			{
			
				nmea("$GPGSA,A,3,06,10,,,17,,,24,,30,,,3.9,3.7,1.0*39");
				nmea("$GPGSA,A,3,16,15,21,18,,,,,,,,,25.0,11.6,22.1*0B");
				nmea("$GPGSA,A,3,16,15,21,18,06,,,,,,,,6.9,4.0,5.6*35");

				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPGSV,3,3,10,09,18,139,36,04,08,034,00*7D");

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051936,A*30";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051938,A*3E";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051940,A*31";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051942,A*33";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051944,A*35";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051946,A*37";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051948,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051950,A*30";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051952,A*32";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051954,A*34";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051956,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.015,N,07953.247,W,051958,A*38";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.048,N,07953.247,W,052000,A*37";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.096,N,07953.247,W,052002,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.144,N,07953.247,W,052004,A*3E";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.117,N,07953.323,W,052006,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.103,N,07953.386,W,052008,A*3D";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.088,N,07953.449,W,052010,A*32";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.073,N,07953.513,W,052012,A*3A";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.058,N,07953.576,W,052014,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.043,N,07953.639,W,052016,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.028,N,07953.702,W,052018,A*3C";
				nmea(gpsdata);

				gpsdata="$GPGLL,4339.013,N,07953.766,W,052020,A*3D";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.998,N,07953.829,W,052022,A*30";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.983,N,07953.892,W,052024,A*3C";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.968,N,07953.956,W,052026,A*32";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.953,N,07954.019,W,052028,A*31";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.938,N,07954.082,W,052030,A*37";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.923,N,07954.145,W,052032,A*35";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.908,N,07954.209,W,052034,A*31";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.893,N,07954.272,W,052036,A*3C";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.878,N,07954.335,W,052038,A*35";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.863,N,07954.399,W,052040,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.848,N,07954.462,W,052042,A*3E";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.833,N,07954.525,W,052044,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.818,N,07954.588,W,052046,A*3A";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.803,N,07954.652,W,052048,A*3A";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.788,N,07954.715,W,052050,A*3D";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.773,N,07954.778,W,052052,A*30";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.758,N,07954.841,W,052054,A*3A";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.743,N,07954.905,W,052056,A*33";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.728,N,07954.968,W,052058,A*3B";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.713,N,07955.031,W,052100,A*3B";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.698,N,07955.095,W,052102,A*35";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.683,N,07955.158,W,052104,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.668,N,07955.221,W,052106,A*33";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.653,N,07955.284,W,052108,A*3A";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.638,N,07955.348,W,052110,A*3F";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.623,N,07955.411,W,052112,A*3C";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.609,N,07955.474,W,052114,A*31";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.594,N,07955.537,W,052116,A*32";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.579,N,07955.601,W,052118,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.564,N,07955.664,W,052120,A*3D";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.549,N,07955.727,W,052122,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.534,N,07955.791,W,052124,A*37";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.519,N,07955.854,W,052126,A*3C";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.504,N,07955.917,W,052128,A*38";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.489,N,07955.980,W,052130,A*3B";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.474,N,07956.044,W,052132,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.459,N,07956.107,W,052134,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.444,N,07956.170,W,052136,A*38";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.429,N,07956.233,W,052138,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.414,N,07956.297,W,052140,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.399,N,07956.360,W,052142,A*3F";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.384,N,07956.423,W,052144,A*35";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.369,N,07956.486,W,052146,A*3B";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.354,N,07956.550,W,052148,A*31";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.339,N,07956.613,W,052150,A*37";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.324,N,07956.676,W,052152,A*3A";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.309,N,07956.739,W,052154,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.294,N,07956.803,W,052156,A*38";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.279,N,07956.866,W,052158,A*36";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.264,N,07956.929,W,052200,A*3E";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.249,N,07956.992,W,052202,A*33";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.234,N,07957.056,W,052204,A*3F";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.218,N,07957.119,W,052206,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.203,N,07957.182,W,052208,A*3F";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.188,N,07957.245,W,052210,A*3E";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.173,N,07957.309,W,052212,A*31";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.158,N,07957.372,W,052214,A*32";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.144,N,07957.435,W,052216,A*39";
				nmea(gpsdata);

				gpsdata="$GPGLL,4338.129,N,07957.498,W,052218,A*3B";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,76.0,M,32.5,M,82.7,M*16";
				nmea(gpsdata);

				gpsdata="$PGRMZ,111,f,2*1B";
				nmea(gpsdata);

				gpsdata="$GPRTE,1,1,c,*37";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,38,209,00*7D";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,43,139,45,19,20,286,00,21,59,095,46,22,21,169,38*7E";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,48,29,24,051,41,37,17,179,00,39,16,175,00*78";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.1,M,69.9,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,120,f,2*19";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,38,209,00*7D";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,43,139,45,19,20,286,00,21,59,095,46,22,21,169,38*7E";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,48,29,24,051,41,37,17,179,00,39,16,175,00*78";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.1,M,69.9,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,127,f,2*1E";
				nmea(gpsdata);

				gpsdata="$GPRTE,1,1,c,*37";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,37,209,00*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,43,139,45,19,20,286,00,21,59,095,45,22,21,169,38*7D";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,47,29,24,051,41,37,17,179,00,39,16,175,00*77";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.1,M,69.9,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,132,f,2*1A";
				nmea(gpsdata);

				gpsdata="$GPRTE,1,1,c,*37";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,37,209,00*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,43,139,45,19,20,286,00,21,59,095,45,22,21,169,37*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,47,29,24,051,41,37,17,179,00,39,16,175,00*77";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.1,M,69.9,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,137,f,2*1F";
				nmea(gpsdata);

				gpsdata="$GPRTE,1,1,c,*37";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,37,209,00*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,43,139,45,19,20,286,00,21,59,095,45,22,21,169,37*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,47,29,24,051,41,37,17,179,00,39,16,175,00*77";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.1,M,69.9,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,142,f,2*1D";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,37,209,00*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,43,139,45,19,20,286,00,21,59,095,45,22,21,169,36*73";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,47,29,24,051,41,37,17,179,00,39,16,175,00*77";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.1,M,69.9,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,148,f,2*17";
				nmea(gpsdata);

				gpsdata="$GPRTE,1,1,c,*37";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,37,209,00*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,44,138,45,19,20,286,00,21,59,095,45,22,21,169,35*76";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,47,29,24,051,41,37,17,179,00,39,16,175,00*77";
				nmea(gpsdata);

				gpsdata="$GPBOD,,T,,M,,*47";
				nmea(gpsdata);

				gpsdata="$PGRME,65.6,M,24.0,M,69.8,M*1A";
				nmea(gpsdata);

				gpsdata="$PGRMZ,153,f,2*1D";
				nmea(gpsdata);

				gpsdata="$GPRTE,1,1,c,*37";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,1,12,03,51,271,00,08,15,353,00,15,59,213,00,16,37,209,00*72";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,2,12,18,44,138,45,19,20,286,00,21,59,095,44,22,21,169,34*76";
				nmea(gpsdata);

				gpsdata="$GPGSV,3,3,12,26,22,063,47,29,24,051,41,37,17,179,00,39,16,175,00*77";
				nmea(gpsdata);
				//------------------------------------------------------------------------------
			
				nmea("$GPRMC,180106,A,6445.5923,N,02056.5368,E,0.0,315.9,121004,6.1,E,A*1D");

				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");

				nmea("$GPRMB,A,,,,,,,,,,,,A,A*0B");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPGSA,A,3,06,10,,,17,,,24,,30,,,3.9,3.7,1.0*39");
				nmea("$GPGSA,A,3,16,15,21,18,,,,,,,,,25.0,11.6,22.1*0B");
				nmea("$GPGSA,A,3,16,15,21,18,06,,,,,,,,6.9,4.0,5.6*35");

				//------------------------------------------------------------------------------

				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");
				nmea("$GPGGA,180106,6445.5923,N,02056.5368,E,1,05,3.7,47.0,M,24.3,M,,*7C");
				nmea("$GPRMC,130405.647,A,4333.2855,N,00130.5644,E,0.00,,040205,,*14");
				nmea("$GPVTG,309.62,T,,M,0.13,N,0.2,K*6E");

			}
			catch(Exception ex)
			{
				OnError(ex,"Error in DemoIt",gpsdata);

			}
		}
		public void Start()
		{
			if (state!=States.Stopped) return;

			thrd = new Thread(new ThreadStart(run));
			thrd.Priority=ThreadPriority.BelowNormal;
			thrd.Name="GPS_Lib";
			thrd.Start();

		}

		public void Stop()
		{
			if (state!=States.Running) return;
		
			if (!demomodeon)
			{
				// signal for the thread to exit
				setstate=States.Stopping;
			}
			else
			{
				// signal for the thread to exit
				setstate=States.Stopped;
			}
		}

		/// <summary>
		/// calculate distance between a position and another position (in meters)
		/// </summary>
		/// <param name="pos1">Position 1</param>
		/// <param name="pos2">Position 2</param>
		/// <param name="unit">Units of measure</param>
		/// <returns></returns>
		public decimal CalculateDistance(Position pos1, Position pos2, Units unit)
		{
			double lat1 = (double)pos1.Latitude_Decimal;
			double lat2 = (double)pos2.Latitude_Decimal;
			double lon1 = (double)pos1.Longitude_Decimal;
			double lon2 = (double)pos2.Longitude_Decimal;
			double distance;
	
			if ((lat1 == lat2) && (lon1 == lon2)) 
				return 0;

			double DEG2RAD = Math.PI/180;
			lat1 *= DEG2RAD;
			lat2 *= DEG2RAD;
			lon1 *= DEG2RAD;
			lon2 *= DEG2RAD;
			distance = (60.0 * ((Math.Acos((Math.Sin(lat1) * Math.Sin(lat2)) + (Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)))) / DEG2RAD));			
			
			switch (unit)
			{
				case Units.Kilometers:
					return Misc.NmToMeters((decimal)distance)/1000;
				case Units.Knots:
					return Misc.ToDecimal((decimal)distance)/1000;
				case Units.Miles:
					return Misc.NmToMiles((decimal)distance)/1000;
				default:
					return 0;
			}
		}
    
		/// <summary>
		/// calculate distance between last position and new position (in km)
		/// </summary>
		/// <returns></returns>
		/// 
		public decimal CalculateDistance(Units unit)
		{
			// if no move, no distance to add
			if (mov.SpeedKnots==0)
				return 0;

			double lat1mem=(double)pos.Latitude_Decimal_Mem;
			double lat2=(double)pos.Latitude_Decimal;
			double lon1mem=(double)pos.Longitude_Decimal_Mem;
			double lon2=(double)pos.Longitude_Decimal;

			// if no move, no distance to add
			if ((lat2 == lat1mem) && (lon2 == lon1mem)) 
				return 0;

			double lat1;
			double lon1;

			// the first time 
			if (InitDistance==true) 
				lat1 = lat1mem;
			else
				lat1 = lat2;
			
			pos.Latitude_Decimal_Mem = pos.Latitude_Decimal;
			
			if (InitDistance==true)
				lon1 = lon1mem;
			else
				lon1 = lon2;

			pos.Longitude_Decimal_Mem = pos.Longitude_Decimal;

			double distance;

			if (InitDistance==false) InitDistance=true;

			if ((lat1 == lat2) && (lon1 == lon2)) 
				return 0;

			double DEG2RAD = Math.PI/180;
			lat1 *= DEG2RAD;
			lat2 *= DEG2RAD;
			lon1 *= DEG2RAD;
			lon2 *= DEG2RAD;
			distance = (60.0 * ((Math.Acos((Math.Sin(lat1) * Math.Sin(lat2)) + (Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)))) / DEG2RAD));			

			switch (unit)
			{
				case Units.Kilometers:
					return Misc.NmToMeters((decimal)distance)/1000;
				case Units.Knots:
					return Misc.ToDecimal((decimal)distance)/1000;
				case Units.Miles:
					return Misc.NmToMiles((decimal)distance)/1000;
				default:
					return 0;
			}
		}
		/// <summary>
		/// calculate distance with time and average speed (in km)
		/// </summary>
		/// <param name="TimePC"></param>
		/// <param name="TimeSAT"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		public decimal CalculateDistance(double TimePC,double TimeSAT, Units unit)
		{
			if (mov.SpeedKnotsAverage==0) return 0;

			double TimeInterval;

			if (pos.SatTime.TimeOfDay.TotalSeconds!=0)
				TimeInterval = pos.SatTime.TimeOfDay.TotalSeconds - TimeSAT;
			else
				TimeInterval = DateTime.Now.TimeOfDay.TotalSeconds - TimePC;
			switch (unit)
			{
				case Units.Kilometers:
					return (Misc.ToDecimal(TimeInterval) * mov.SpeedKphAverage)/3600;
				case Units.Knots:
					return (Misc.ToDecimal(TimeInterval) * mov.SpeedKnotsAverage)/3600;
				case Units.Miles:
					return (Misc.ToDecimal(TimeInterval) * mov.SpeedMphAverage)/3600;
				default:
					return 0;
			}
		}

		/// <summary>
		/// calculate bearing between a position and another position (in degrees)
		/// </summary>
		/// <param name="pos1"></param>
		/// <param name="pos2"></param>
		/// <returns></returns>
		public decimal CalculateBearing(Position pos1,Position pos2)
		{
			double DEG2RAD = Math.PI/180;
			double lat1 = (double)pos1.Latitude_Decimal * DEG2RAD;
			double lat2 = (double)pos2.Latitude_Decimal * DEG2RAD;
			double lon1 = (double)pos1.Longitude_Decimal * DEG2RAD;
			double lon2 = (double)pos2.Longitude_Decimal * DEG2RAD;
			double bearing;
	
			if ((lat1 == lat2) && (lon1 == lon2)) 
				return 0;

			bearing = (Math.Atan2(Math.Sin(lon2 - lon1) * Math.Cos(lat2),
				Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1))) / DEG2RAD;
			// returns a value between -180 and 180.
			if (bearing < 0.0)
				bearing += 360.0;

			return (decimal)bearing;
		}
    
		#endregion

		#region protected methods
		protected virtual void OnGpsSentence(GpsSentenceEventArgs e)
		{
			if (GpsSentence != null) GpsSentence(this, e);
		}

		protected virtual void OnGpsCommState(GpsCommStateEventArgs e)
		{
			if (GpsCommState!=null) GpsCommState(this, e);
		}

		protected virtual void OnPosition(Position pos)
		{
			if (Position!=null) Position(this,pos);
		}

		protected virtual void OnMovement(Movement mov)
		{
			if (Movement!=null) Movement(this,mov);
		}

		protected virtual void OnSatellites(Satellite[] satellites)
		{
			if (Satellite!=null) Satellite(this,satellites);
		}

		protected virtual void OnError(Exception exception,string message,string gps_data)
		{
			lasterror=message;
			if (Error!=null) Error(this,exception,message,gps_data);
		}

		protected virtual void OnAutoDiscovery(OpenNETCF.IO.Serial.GPS.AutoDiscoverStates state, string port, OpenNETCF.IO.Serial.BaudRates bauds)
		{
			if (GpsAutoDiscovery!=null) GpsAutoDiscovery(this,state,port,bauds);
		}
		protected virtual void OnDataReceived(string data)
		{
			if (DataReceived!=null) DataReceived(this,data);
		}

		#endregion

		#region events
		public event GpsSentenceEventHandler GpsSentence;
		public event GpsCommStateEventHandler GpsCommState;
		public event PositionEventHandler Position;
		public event MovementEventHandler Movement;
		public event SatelliteEventHandler Satellite;
		public event ErrorEventHandler Error;
		public event GpsAutoDiscoveryEventHandler GpsAutoDiscovery;
		private event DataReceivedEventHandler DataReceived;
		#endregion

		#region delegates
		public delegate void GpsSentenceEventHandler(object sender, GpsSentenceEventArgs e);
		public delegate void GpsCommStateEventHandler(object sender, GpsCommStateEventArgs e);
		public delegate void GpsStatusEventHandler(object sender, StatusType GpsStatus);
		public delegate void PositionEventHandler(object sender,Position pos);
		public delegate void MovementEventHandler(object sender,Movement mov);
		public delegate void SatelliteEventHandler(object sender,Satellite[] satellites);
		public delegate void ErrorEventHandler(object sender,Exception exception,string message,string gps_data);
		public delegate void GpsAutoDiscoveryEventHandler(object sender, OpenNETCF.IO.Serial.GPS.AutoDiscoverStates state, string port, OpenNETCF.IO.Serial.BaudRates bauds);
		private delegate void DataReceivedEventHandler(object sender, string data);
		#endregion

		#region properties
		public bool AutoDiscovery
		{
			set
			{
				autodiscovery=value;
			}
			get
			{
				return autodiscovery;
			}
		}

		public Satellite[] Satellites
		{
			get
			{
				return satellites;
			}
		}

		public Position Pos
		{
			get
			{
				return pos;
			}
		}

		private Movement Mov
		{
			get
			{
				return mov;
			}
		}

		public bool TraceOn
		{
			set
			{
				traceon=value;
			}
			get
			{
				return traceon;
			}
		}

		public bool DemoModeOn
		{
			set
			{
				demomodeon=value;
			}
		}

		public string DemoFile
		{
			set
			{
				demofile=value;
			}
			get
			{
				return demofile;
			}
		}

		public string TraceFile
		{
			set
			{
				tracefile=value;
			}
			get
			{
				return tracefile;
			}

		}

		public States State 
		{
			get
			{
				return state;
			}
		}

		public StatusType GpsState
		{
			get
			{
				return gpsstatus;
			}
		}

		private States setstate
		{
			set
			{
				state=value;
				this.OnGpsCommState(new GpsCommStateEventArgs(value));
			}
		}

		public string ComPort
		{
			set
			{
				comport=value;
			}
		}

		public Serial.BaudRates BaudRate
		{
			set
			{
				baudrate=value;
			}
		}
		public Serial.BaudRates MaxBaudRate
		{
			set
			{
				maxbaudrate=value;
			}
			get
			{
				return maxbaudrate;
			}
		}
		public Serial.BaudRates MinBaudRate
		{
			set
			{
				minbaudrate=value;
			}
			get
			{
				return minbaudrate;
			}
		}

		public string LastError
		{
			get
			{
				return lasterror;
			}
		}

		public Decimal HdopMaxError
		{
			set
			{
				hdopmaxerror=value;
			}
			get
			{
				return hdopmaxerror;
			}
		}

		public Decimal Hdop
		{
		get
			{
				return hdop;
			}
		}

		public Decimal Vdop
		{
			get
			{
				return vdop;
			}
		}

		public Decimal Pdop
		{
			get
			{
				return pdop;
			}
		}

		public int SatInView
		{
			get	
			{
				return nbsatinview;
			}
		}
		public Fix_Mode FixMode
		{
			get	
			{
				return fixmode;
			}
		}
		public Fix_Indicator FixIndicator
		{
			get	
			{
				return fixindicator;
			}
		}
		public Fix_Type FixType
		{
			get	
			{
				return fixtype;
			}
		}
		public Fix_TypeMode FixTypeMode
		{
			get	
			{
				return fixtypemode;
			}
		}

		// TODO : waypoint stuff - not yet fully implemented
		#region waypoint stuff - not yet fully implemented
		public Position WayPoint
		{
			get
			{
				return waypoint;
			}
			set
			{
				waypoint=value;
			}
		}

		public decimal DistanceToWayPoint
		{
			get
			{
				// HELP REQUIRED HERE - to work out distance to waypoint
				calculate_waypoint();
				return distancetowaypoint;
			}
		}
		public decimal BearingToWayPoint
		{
			
			get
			{
				// HELP REQUIRED HERE - to work out distance to waypoint
				calculate_waypoint();
				return bearingtowaypoint;
			}
		}

		public decimal CourseCorrection
		{
			
			get
			{
				// HELP REQUIRED HERE - to work out what course correction to apply to get to waypoint
				calculate_waypoint();
				return coursecorrection;
			}
		}

		#endregion
		
		#endregion

		#region private methods
		private void reset_gps_vars()
		{
			strreceived="";
			sentencecount=0;

			pos = null;
			pos = new Position();
			mov = null;
			mov = new Movement();
			satellites = null;
			lasterror="";

			distancetowaypoint=0;
			bearingtowaypoint=0;
			coursecorrection=0;
			
			InitDistance = false;

			// Position dilution of precision
			pdop=50;

			// Horizontal dilution of precision
			hdop=50;

			// Vertical dilution of precision
			hdop=50;

		}

		// loads demo data into our nmea parser
		private void load_demo_data()
		{
			if (!File.Exists(demofile))
			{
				setstate=States.Stopping;
				return;
			}
			StreamReader demostream = File.OpenText(demofile);
			string str="";
			while ((str=demostream.ReadLine()) != null && state==States.Running)
			{
				//nmea(str);
				OnDataReceived(str);
			}
			setstate=States.Stopped;
		}

		private void calculate_waypoint()
		{
			if (pos==null || waypoint==null) return;
	
			distancetowaypoint = CalculateDistance(pos,waypoint,Units.Kilometers);
			bearingtowaypoint = CalculateBearing(pos,waypoint);
			coursecorrection = bearingtowaypoint - mov.Track;
			if (coursecorrection < -180m)
				coursecorrection += 360m;
			if (coursecorrection > 180m)
				coursecorrection -= 360m;
		}

		private void run()
		{

			setstate=States.Opening;
			this.reset_gps_vars();

			DataReceived+= new DataReceivedEventHandler(GPS_DataReceived);
			if (!demomodeon)
			{
				if (this.autodiscovery)
				{
					if (!autodiscover())
					{
						setstate=States.Stopped;
						return;
					}
				}
			}

			if (!demomodeon)
			{
				if (!isconnecteddevice())
				{
					OnError(null,"Com Port "+comport+" Is Not On Device","");
					setstate=States.Stopped;
					return;
				}
				if (state!=States.AutoDiscovery)
				{
					DetailedPortSettings portSettings = new HandshakeNone(); 
					cp = new Port(comport,portSettings);
					cp.Settings.BaudRate=this.baudrate;
					cp.RThreshold=64;
					cp.InputLen=1;

					try 
					{
						cp.Open();
					}
					catch (Exception e)
					{
						OnError(e,"Could Not Open Com Port "+comport,"");
						cp.Dispose();
						setstate=States.Stopped;
						return;
					}
					cp.OnError+=new Port.CommErrorEvent(cp_OnError);
					cp.DataReceived+=new Port.CommEvent(cp_DataReceived);
				}
			}

			if (state==States.Opening || state==States.AutoDiscovery)
				setstate=States.Running;

			if (demomodeon)
			{
				load_demo_data();
			}
			else
			{
				while (state==States.Running)
				{
					Thread.Sleep(10);
				}

				if (cp.IsOpen) cp.Close();
				cp.Dispose();
				cp=null;
			}
			setstate=States.Stopped;
		}

		/// <summary>
		/// this is fired when the comport receives data
		/// </summary>
		private void cp_DataReceived()
		{
			try
			{
				if (state!=States.Running) return;
			
				string strret="";
				string strdata="";
				
				byte[] inputData;
			
				while (cp.InBufferCount > 0)
				{

					inputData = cp.Input;
					strret = Encoding.ASCII.GetString(inputData, 0,1);
					if (strret=="\n") // If newline
					{
						strdata=this.strreceived.Substring(0,strreceived.Length-1);
						//nmea(strdata);
						OnDataReceived(strdata);

						strdata="";
						strreceived="";
						if (autodiscovery==true)
						{
							setstate=States.AutoDiscovery;
						}
					}					
					else
						strreceived+=strret;	
				}
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in cp_DataReceived","");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		private void GPS_DataReceived(object sender, string data)
		{
			nmea(data);
		}
		/// <summary>
		/// processes our string of nmea data
		/// </summary>
		/// <param name="gps_data">string of input data</param>
		/// <returns>Boolean to indicate if we were able to process string of data ok</returns>
		private bool nmea(string gps_data) 
		{
			try
			{

			// GPS data can't be zero length
			if (gps_data.Length==0) return false;

			// first character must be a $
			if (gps_data[0]!='$') return false;

			// GPS data can't be longer than 82 character
			if (gps_data.Length>82) return false;

			// remove our leading character
			string strdata=gps_data.Substring(1);

			// see if the last block contains a * used to see if we have a checksum
			int intstarpos=strdata.IndexOf('*');
			if (intstarpos>=0)
			{
				// we have a checksum so check it...
				string strchecksum=strdata.Substring(intstarpos+1);

				// remove checksum from end of string
				strdata=strdata.Substring(0,strdata.Length-strchecksum.Length-1);

				if (!checksum(strdata,strchecksum))
				{
					OnError(null,"Checksum failed on: '"+gps_data+"'",gps_data);
					return false;
				}
			}

			String[] strrecarray = strdata.Split(',');

			// get the first block which is the sentence id
			string strsentence=strrecarray[0];

			ArrayList sbdata = new ArrayList();

			// get the data block minus the first block
			for (int i=1;i<strrecarray.Length;i++)
				sbdata.Add(strrecarray[i]);

			// if all is well raise our main GPS event
			string[] arrydata = (string[]) sbdata.ToArray(typeof(string));

			sbdata = null;

			// increment our counter
			if (sentencecount==int.MaxValue) sentencecount=0;

			sentencecount++;
			OnGpsSentence(new GpsSentenceEventArgs(gps_data,sentencecount));

			if (traceon)
			{
				StreamWriter sw = File.AppendText(tracefile);
				sw.WriteLine(gps_data);
				sw.Close();
			}

			// simple checksums to see if we need to fire and event
			//decimal poscheck = pos.Latitude_Fractional + pos.Longitude_Fractional;
			//decimal movcheck = mov.SpeedKnots+mov.Track;
			StatusType oldgpsstatus = this.gpsstatus;
			switch (strsentence)
			{
				case "GPRMC": // Recommended minimum specific GPS/Transit data 
					//if (storedgprmc!=gps_data)
					//{
						//storedgprmc=gps_data;
						process_gprmc(arrydata,gps_data);
					//}
					break;
				case "GPGGA": //Global positioning system fixed data
					//if (storedgpgga!=gps_data)
					//{
						//storedgpgga=gps_data;
						process_gpgga(arrydata,gps_data);
					//}
					break;

				case "GPGSA": // GPS DOP and active satellites

					//if (storedgpgsa!=gps_data)
					//{

						//storedgpgsa=gps_data;
						process_gpgsa(arrydata,gps_data);
					//}
					break;

				case "GPGSV": // Satellites in view
					//if (storedgpgsv!=gps_data)
					//{
						//storedgpgsv=gps_data;
						if (process_gpgsv(arrydata,gps_data))
							OnSatellites(this.satellites);
					//}
					break;
				case "GPGLL": // Location Fix
					//if (storedgpgll!=gps_data)
					//{
						//storedgpgll=gps_data;
						process_gpgll(arrydata,gps_data);
					//}
					break;
				case "GPVTG": // 
					//if (storedgpvtg!=gps_data)
					//{
						//storedgpvtg=gps_data;
						process_gpvtg(arrydata,gps_data);
					//}
					break;
				default: 

					return true;

			}
			// TODO poscheck not a good solution
			// see if our long and lat have changed
			//if (poscheck != (pos.Latitude_Fractional  + pos.Longitude_Fractional ))
			//{
				// only fire if we have under the HDOP maxium error
				//if (hdop<=hdopmaxerror)
					//OnFix(pos,fixtype);
				OnPosition(pos);
				//}
			// TODO movcheck not a good solution
			//if (movcheck!=mov.SpeedKnots+mov.Track)
			//{
				// only fire if we have under the HDOP maxium error
				//if (hdop<=hdopmaxerror)
				OnMovement(mov);
			//}

//			if (oldgpsstatus!=gpsstatus)
//			{
//				OnGpsStatus(new GpsStatusEventArgs(gpsstatus));
//			}
			return true;

			}
			catch(Exception ex)
			{
				OnError(ex,"Error in nmea",gps_data);
				return false;
			}
		}

		/// <summary>
		/// this checksums all nmea sequences
		/// </summary>
		/// <param name="strtocheck">string to check</param>
		/// <param name="strchecksum">checksum</param>
		/// <returns>true if checksum computes</returns>
		private bool checksum(string strtocheck,string strchecksum)
		{
			int intor=0;
			// go from first character upto last *
			for(int i=0;(i<strtocheck.Length);i++)
				intor=intor^ (int) (strtocheck[i]);

			int y = 0;

			try 
			{
				y = Convert.ToInt32(strchecksum,16);
			}
			catch
			{
				return false;
			}
			if (intor != y)
			{
				// debug for checksum failures
				intor+=0;
			}
			return (intor == y);
		}

		private void cp_OnError(string Description)
		{
			if (state!=States.Running) return;
			OnError(null,"Com Port Error Received "+Description,"");
			this.setstate=States.Stopping;
		}

		

		private bool isconnecteddevice()
		{
			
			foreach (string port in this.devices())
			{
			
				if (port.ToUpper()==this.comport.ToUpper())
				{
					return true;
				}
			}
			return false;
		}

		private string[] devices()
		{
			// lists all modems on the device
			ArrayList al = new ArrayList();
			RegistryKey defkey;
			string[] keyNames;
			string keyvalue="";
			string strport="";
		
			// now get the active
			defkey=Registry.LocalMachine.CreateSubKey(@"Drivers\Active");
			
			keyNames =defkey.GetSubKeyNames();
			foreach (string x in keyNames)
			{
				
				try	
				{
					keyvalue=defkey.CreateSubKey(x).GetValue("Key").ToString();
					strport=defkey.CreateSubKey(x).GetValue("Name").ToString();
					
					// fudge to remove rubbish off the end of the string
                    // AS-20160713: fixing up
                    var ix = strport.IndexOf("'");
                    if (ix >= 0)
    					strport = strport.Substring(0,ix);

					RegistryKey thekey=Registry.LocalMachine.CreateSubKey(keyvalue);
					
					if (strport.StartsWith("COM"))
						al.Add(strport);
				}
				catch
				{

				}
			}
			return (string[]) al.ToArray(typeof(string));
		}

		/// <summary>
		/// Auto discover our GPS
		/// </summary>
		/// <returns>True if we have found a GPS</returns>
		private bool autodiscover()
		{
			Type searchbauds = typeof(OpenNETCF.IO.Serial.BaudRates);
							
			// get a list of devices
			foreach (string port in this.devices())
			{
				foreach (OpenNETCF.IO.Serial.BaudRates bauds in OpenNETCF.EnumEx.GetValues(searchbauds))
				{
					if ((bauds >= minbaudrate) && (bauds <= maxbaudrate))
					{
						// initialize the port like in run proc
						DetailedPortSettings portSettings = new HandshakeNone(); 
						cp = new Port(port,portSettings);
						cp.Settings.BaudRate = bauds;

						cp.RThreshold=64;
						cp.InputLen=1;
						OnAutoDiscovery( OpenNETCF.IO.Serial.GPS.AutoDiscoverStates.Testing,port,cp.Settings.BaudRate);

						try
						{
							cp.Open();
						}
						catch
						{
							OnAutoDiscovery(OpenNETCF.IO.Serial.GPS.AutoDiscoverStates.FailedToOpen,port,cp.Settings.BaudRate);
						}

						if (cp.IsOpen)
						{
							// if port open, invoke cp_DataReceived if bytes occur
							cp.DataReceived+=new Port.CommEvent(cp_DataReceived);

							if (state==States.Opening)
								setstate=States.Running;
					
							int cpt=0;
							while (state==States.Running && cpt <= 100)
							{
								// wait 1 seconds for the sentences
								Thread.Sleep(10);
								cpt++;
							}

							if (state==States.AutoDiscovery)
							{
								OnAutoDiscovery(OpenNETCF.IO.Serial.GPS.AutoDiscoverStates.Opened,port,cp.Settings.BaudRate);
								comport = port;
								baudrate = cp.Settings.BaudRate;
								autodiscovery=false;
								return true;
							}
							else
							{
								cp.Close();
								setstate=States.Opening;
							}

							OnAutoDiscovery(OpenNETCF.IO.Serial.GPS.AutoDiscoverStates.Failed,port,cp.Settings.BaudRate);
						}

						cp.Dispose();
						cp=null;
					}
				}
			}
			// default values
			OnAutoDiscovery(OpenNETCF.IO.Serial.GPS.AutoDiscoverStates.NoGPSDetected,comport,baudrate);
			autodiscovery=false;
			return false;
		}

		#endregion

		#region gps stored data
		private StatusType gpsstatus=StatusType.Warning; 
		private Position pos=null;
		private Movement mov=null;
		private Satellite[] satellites = null;
		#endregion 

		#region gpsmessage handlers
		private bool process_gpgsv(string []strdata, string gpsdata)
		{	
			try
			{
				
				int numberofmessages=0;
				int messagenumber=0;
				
				//int numsat=0;
				// 0 Number of messages 3 Number of messages in complete message (1-3)
				if (strdata[0].Length>0)
				{
					numberofmessages=Convert.ToInt32(strdata[0]);
				}

				// 1 Sequence number 1 -Sequence number of this entry (1-3)
				if (strdata[1].Length>0)
				{
					messagenumber=Convert.ToInt32(strdata[1]);
				}
				// 2 Satellites in view - 10
				if (strdata[2].Length>0)
				{
					nbsatinview=Convert.ToInt32(strdata[2]);
					if (satellites==null)
					{
						setupsats(nbsatinview);
					}
				}

				if (messagenumber==0||nbsatinview==0) return false;

				int whichsat=(nbsatinview-((messagenumber-1)*4)-1);
				int sats;
				int intcount=0;


				if (whichsat >= 4)
					sats = 4;
				else
					sats = nbsatinview-((messagenumber-1)*4);


				for (int i=1;i<=sats;i++) 
				{
					// 3 Satellite ID 1 - 20- Range is 1-32
					if (strdata[intcount+3].Length>0)
						satellites[whichsat].ID=Convert.ToInt32(strdata[intcount+3]);

					// 4 Elevation 1- 78- Elevation in degrees (0-90)
					if (strdata[intcount+4].Length>0)
						satellites[whichsat].Elevation=Convert.ToInt32(strdata[intcount+4]); 

					// 5 Azimuth 1 - 331- Azimuth in degrees (0-359)
					if (strdata[intcount+5].Length>0)
						satellites[whichsat].Azimuth=Convert.ToInt32(strdata[intcount+5]);

					// 6 SNR 1 -45 - Signal to noise ration in dBHZ (0-99)
					if (strdata[intcount+6].Length>0)
						satellites[whichsat].SNR=Convert.ToInt32(strdata[intcount+6]);
					else
						satellites[whichsat].SNR=0;

					intcount += 4;
					whichsat--;	
				}
				return (numberofmessages==messagenumber);
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in process_gpgsv",gpsdata);
				return false;
			}
		}
		private void process_gpgsa(string []strdata,string gpsdata)
		{
			try
			{
				//0 Mode 1 - A - A = Auto 2D/3D, M = Forced 2D/3D
				if (strdata[0].Length>0)
				{
					if (strdata[0][0]=='A')
						fixmode = OpenNETCF.IO.Serial.GPS.Fix_Mode.Auto;
					else
						fixmode = OpenNETCF.IO.Serial.GPS.Fix_Mode.Manual;
				}

				//1 Mode 1 - 3 - 1 = No fix, 2 = 2D, 3 = 3D
				if (strdata[1].Length>0)
				{
					switch (Convert.ToInt32(strdata[1]))
					{
						case 1:
							fixindicator=OpenNETCF.IO.Serial.GPS.Fix_Indicator.NotSet;
							fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NotSet;
							break;
						case 2:
							fixindicator=OpenNETCF.IO.Serial.GPS.Fix_Indicator.Mode2D;
							fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NoAltitude;
							break;
						case 3:
							fixindicator=OpenNETCF.IO.Serial.GPS.Fix_Indicator.Mode3D;
							fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.WithAltitude;
							break;
						default:
							fixindicator=OpenNETCF.IO.Serial.GPS.Fix_Indicator.NotSet;
							fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NotSet;
							break;
					}
				}

				// if satellites is null because GSV sentence not arrived or not present
				if (satellites==null)
				{
					nbsatused=0;
					for (int intcount=0;intcount<12;intcount++)
					{
						string strid=strdata[2+intcount];
						if (strid!="")
						{
							nbsatused++;
						}
					}
					setupsats(nbsatused);
				}


				if (satellites!=null)
				{
					foreach (Satellite s in this.satellites)
					{
						s.Active=false;
						s.Channel=0;
					}

					//  2 Satellite used 1  - 01 - Satellite used on channel  1
					//  3 Satellite used 2  - 20 - Satellite used on channel  2
					//  4 Satellite used 3  - 19 - Satellite used on channel  3
					//  5 Satellite used 4  - 13 - Satellite used on channel  4
					//  6 Satellite used 5  -    - Satellite used on channel  5
					//  7 Satellite used 6  -    - Satellite used on channel  6
					//  8 Satellite used 7  -    - Satellite used on channel  7
					//  9 Satellite used 8  -    - Satellite used on channel  8
					// 10 Satellite used 9  -    - Satellite used on channel  9
					// 11 Satellite used 10 -    - Satellite used on channel 10
					// 12 Satellite used 11 -    - Satellite used on channel 11
					// 13 Satellite used 12 -    - Satellite used on channel 12

					nbsatused=0;
					for (int intcount=0;intcount<12;intcount++)
					{
						string strid=strdata[2+intcount];
						if (strid!="")
						{
							nbsatused++;
							setsat(Convert.ToInt32(strid),intcount+1);
						}
					}
				}

				// 14 PDOP 40.4 Position dilution of precision
				if (strdata[14].Length>0)
					pdop=Misc.ToDecimal(strdata[14]);
		
				// 15 HDOP 24.4 Horizontal dilution of precision
				if (strdata[15].Length>0)
					hdop=Misc.ToDecimal(strdata[15]);
					
				
				// 16 VDOP 32.2 Vertical dilution of precision
				if (strdata[16].Length>0)
					vdop=Misc.ToDecimal(strdata[16]);
				
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in process_gpgsa",gpsdata);
			}
		}

		/// <summary>
		/// sets the id's in our list of satellites
		/// </summary>
		/// <param name="id"></param>
		/// <param name="channel"></param>
		private void setsat(int id,int channel)
		{
			if (id==0) return;
			bool satfound=false;

			foreach (Satellite s in this.satellites)
			{
				if (s.ID==id)
				{
					s.Active=true;
					s.Channel=channel;
					satfound=true;
					break;
				}
			}
			if (satfound==false)
			{
				foreach (Satellite s in this.satellites)
				{
					if (s.ID==0)
					{
						s.ID=id;
						s.Active=true;
						s.Channel=channel;
						satfound=true;
						break;
					}
				}
			}

		}
		/// <summary>
		/// sets up our array of sats
		/// </summary>
		/// <param name="numsats"></param>
		private void setupsats(int numsats)
		{
			if (numsats==0) return;
			// TODO it will be better to have an arraylist
			satellites = new Satellite[30];

			for (int intcount=0;intcount<satellites.Length;intcount++)
			{
					satellites[intcount]= new Satellite();
			}
		}

		private void process_gpgga(string []strdata,string gpsdata)
		{
			try
			{
				//0 UTC Time
				if (strdata[0].Length>0)
				{
					settime(strdata[0]);
				}

				//1 Latitude
				if (strdata[1].Length>0)
				{
					pos.Latitude_Fractional=Misc.ToDecimal(strdata[1]);
				}
			
				//2 N/S Indicator
				if (strdata[2].Length>0)
				{
					if (strdata[2][0]=='N')
						pos.DirectionLatitude=CardinalDirection.North;
					else
						pos.DirectionLatitude=CardinalDirection.South;
				}

				//3 Longitude
				if (strdata[3].Length>0)
				{
					pos.Longitude_Fractional=Misc.ToDecimal(strdata[3]);
				}

				//4 E/W Indicator
				if (strdata[4].Length>0)
				{
					if (strdata[4][0]=='W')
						pos.DirectionLongitude=CardinalDirection.West;
					else
						pos.DirectionLongitude=CardinalDirection.East;
				}

				//5 Position Fix - 0 = Invalid, 1 = Valid SPS, 2 = Valid DGPS, 3 = Valid PPS, 4 = Valid RTK
				if (strdata[5].Length>0)
				{
					switch (Convert.ToInt32(strdata[5]))
					{
						case 0:
							fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.NotSet;
							//fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NotSet;
							break;
						case 1:
							fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.SPS;
							//fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.WithAltitude;
							break;
						case 2:
							fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.DSPS;
							//fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.WithAltitude;
							break;
						case 3:
							fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.PPS;
							//fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.WithAltitude;
							break;
						case 4:
							fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.RTK;
							//fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.WithAltitude;
							break;
						default:
							fixtypemode=OpenNETCF.IO.Serial.GPS.Fix_TypeMode.NotSet;
							//fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NotSet;
							break;
					}
				}

				//6 Satellites Used (0-12)
				if (strdata[6].Length>0)
				{
					nbsatused=Convert.ToInt32(strdata[6]);
				}

				//7 HDOP- Horizontal dilution of precision
				if (strdata[7].Length>0)
					hdop=Misc.ToDecimal(strdata[7]);

				//8 Altitude- Altitude in meters according to WGS-84 ellipsoid
				if (strdata[8].Length>0)
				{
						pos.Altitude=Misc.ToDecimal(strdata[8]);
				}

				//9 Altitude Units - M = Meters
				//if (strdata[9].Length>0)
				//{
				// Not necessary
				//}

				//10 Geoid Seperation- Geoid seperation in meters according to WGS-84 ellipsoid
				if (strdata[10].Length>0)
				{
					pos.GeoidSeparation = Misc.ToDecimal(strdata[10]);
				}

				//11 Geoid Seperation Units - M = Meters
				//if (strdata[11].Length>0)
				//{
				// Not necessary
				//}

				//12 DGPS Age - Age of DGPS data in seconds
				//if (strdata[12].Length>0)
				//{
				// To be implemented
				//}

				//13 DGPS Station ID-0000
				//if (strdata[13].Length>0)
				//{
				// To be implemented
				//}
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in process_gpgga",gpsdata);
			}
		}

		private void process_gpgll(string []strdata,string gpsdata)
		{
			try
			{
				// 0 Latitude 4250.5589 ddmm.mmmm 
				if (strdata[0].Length>0)
				{
					pos.Latitude_Fractional=Misc.ToDecimal(strdata[0]);
				}

				// 1 N/S Indicator S N = North, S = South 
				if (strdata[1].Length>0)
				{
					if (strdata[1][0]=='N')
						pos.DirectionLatitude=CardinalDirection.North;
					else
						pos.DirectionLatitude=CardinalDirection.South;
				}

				// 2 Longitude 14718.5084 dddmm.mmmm 
				if (strdata[2].Length>0)
				{
					pos.Longitude_Fractional=Misc.ToDecimal(strdata[2]);
				}

				// 3 E/W Indicator E E = East, W = West
				if (strdata[3].Length>0)
				{
					if (strdata[3][0]=='W')
						pos.DirectionLongitude=CardinalDirection.West;
					else
						pos.DirectionLongitude=CardinalDirection.East;
				}


				// 4 UTC Time HHMMSS 
				if (strdata[4].Length>0)
				{
					settime(strdata[4]);
				}
				
				// 5 Status A A = Valid, V = Invalid 
				gpsstatus=(strdata[5]=="A")?StatusType.OK:StatusType.Warning;
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in process_gpgll",gpsdata);
			}
		}

		private void process_gprmc(string []strdata,string gpsdata)
		{
			try
			{
				// 0 180432 UTC of position fix in hhmmss format (18 hours, 4 minutes and 32 seconds)
				if (strdata[0].Length>0)
				{
					settime(strdata[0]);
				}

				// 1 A Status (A - data is valid, V - warning) 
				gpsstatus=(strdata[1]=="A")?StatusType.OK:StatusType.Warning;
				/*
				switch (strdata[1])
				{
					case "A":
						fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.WithAltitude;
						break;
					case "V":
						fixtype=OpenNETCF.IO.Serial.GPS.Fix_Type.NotSet;
						break;
				}
				*/

				//2 4027.027912 Geographic latitude in ddmm.mmmmmm format (40 degrees and 27.027912 minutes) 
				if (strdata[2].Length>0)
				{
					pos.Latitude_Fractional=Misc.ToDecimal(strdata[2]);
				}		
			

				// 3 N Direction of latitude (N - North, S - South) 
				if (strdata[3].Length>0)
				{
					if (strdata[3][0]=='N')
						pos.DirectionLatitude=CardinalDirection.North;
					else
						pos.DirectionLatitude=CardinalDirection.South;
				}

				// 4 08704.857070 Geographic longitude in dddmm.mmmmmm format (87 degrees and 4.85707 minutes) 
				if (strdata[4].Length>0)
				{
					pos.Longitude_Fractional=Misc.ToDecimal(strdata[4]);
				}

				// 5 W Direction of longitude (E - East, W - West) 

				if (strdata[5].Length>0)
				{
					if (strdata[5][0]=='W')
						pos.DirectionLongitude=CardinalDirection.West;
					else
						pos.DirectionLongitude=CardinalDirection.East;
				}

				//6 000.04 Speed over ground (0.04 knots) 
				if (strdata[6].Length>0)
				{
					mov.SpeedKnots=Misc.ToDecimal(strdata[6]);
					mov.NbSpeedValues = mov.NbSpeedValues + 1;
				}


				// 7 181.9 Track made good (heading) (181.9�) 
				if (strdata[7].Length>0)
					mov.Track=Misc.ToDecimal(strdata[7]);

				//8 131000 Date in ddmmyy format (October 13, 2000) 
				if (strdata[8].Length>0)
				{
					setdate(strdata[8], strdata[0]);
				}

				// 9 1.8 Magnetic variation (1.8�) 
				if (strdata[9].Length>0)
					mov.MagneticVariation=Misc.ToDecimal(strdata[9]);

				//10 W Direction of magnetic variation (E - East, W - West) 
				if (strdata[10].Length>0)
				{
					if (strdata[10][0]=='W')
						mov.DirectionMagnetic=CardinalDirection.West;
					else
						mov.DirectionMagnetic=CardinalDirection.East;
				}

			}
			catch(Exception ex)
			{
				OnError(ex,"Error in process_gprmc",gpsdata);
			}
		}

		private void process_gpvtg(string []strdata,string gpsdata)
		{
			try
			{
				// 0 309.62 degrees Course over ground - Measured heading
				if (strdata[0].Length>0)
				{
					mov.Track=Misc.ToDecimal(strdata[0]);
				}

				// 1 T Course Reference - True
				//if (strdata[1].Length>0)
				//{
					// Not necessary
				//}
 
				//2 xxx.xx degrees Course over ground - Measured heading 
				//if (strdata[2].Length>0)
				//  Not necessary as information is available in GPRMC
				//	mov.MagneticVariation=Misc.ToDecimal(strdata[2]);
			
				// 3 T Course Reference - Magnetic
				//if (strdata[3].Length>0)
				//{
					// Not necessary
				//}

				// 4 0.13 Measured horinzontal speed in knots 
				if (strdata[4].Length>0)
				{
					mov.SpeedKnots=Misc.ToDecimal(strdata[4]);
					mov.NbSpeedValues = mov.NbSpeedValues + 1;
				}

				// 5 N for knots 
				//if (strdata[5].Length>0)
				//{
					// Not necessary
				//}

				// 6 0.2 Measured horinzontal speed in kilometers per hours 
				//if (strdata[6].Length>0)
				//{
					// Not necessary, use mov.SpeedKph
				//}

				// 7 K for kilometers per hours 
				//if (strdata[7].Length>0)
				//{
					// Not necessary
				//}

			}
			catch(Exception ex)
			{
				OnError(ex,"Error in process_gpvtg",gpsdata);
			}
		}

		private void settime(string strtime)
		{
			try
			{

				int day = DateTime.Now.Day;
				int month = DateTime.Now.Month;
				int year = DateTime.Now.Year;

				int utchours = Convert.ToInt32(strtime.Substring(0, 2));
				int utcminutes = Convert.ToInt32(strtime.Substring(2, 2));
				int utcseconds = Convert.ToInt32(strtime.Substring(4, 2));
				int utcmilliseconds =0;

				// extract milliseconds if it is available
				if (strtime.Length > 7)
					utcmilliseconds = Convert.ToInt32(strtime.Substring(7));

				// now build a datetime object with all values
				pos.SatTime= new DateTime(year, month, day, utchours, utcminutes, utcseconds, utcmilliseconds);
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in settime",strtime);
			}
		}

		private void setdate(string strdate, string strtime)
		{
			try
			{
				// now build a datetime object with all values
				int day = Convert.ToInt32(strdate.Substring(0, 2));
				int month = Convert.ToInt32(strdate.Substring(2, 2));
				// available for this century
				int year = Convert.ToInt32(strdate.Substring(4, 2)) + 2000;

				int utchours = Convert.ToInt32(strtime.Substring(0, 2));
				int utcminutes = Convert.ToInt32(strtime.Substring(2, 2));
				int utcseconds = Convert.ToInt32(strtime.Substring(4, 2));
				int utcmilliseconds =0;

				// extract milliseconds if it is available
				if (strtime.Length > 7)
					utcmilliseconds = Convert.ToInt32(strtime.Substring(7));

				pos.SatDate= new DateTime(year, month, day, utchours, utcminutes, utcseconds, utcmilliseconds);
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in setdate",strdate + " / " + strtime);
			}
		}
		/// <summary>
		/// Send to GPS receiver a sentence like $PSRF103(enable/disable sentences) or $PSRF100(change serial gps speed)
		/// </summary>
		public bool SendGpsMessage(string GPSSentence)
		{
			try
			{
				// Port open
				if (state==States.Running)
				{
					cp.Output = Encoding.ASCII.GetBytes(GPSSentence);
					return true;
				}
				else
					// Port closed
				{
					return false;
				}
			}
			catch(Exception ex)
			{
				OnError(ex,"Error in SendRate ",GPSSentence);
				return false;
			}
		}

		#endregion
	}
}
