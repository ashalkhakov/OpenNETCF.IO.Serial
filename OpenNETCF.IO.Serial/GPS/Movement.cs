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
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace OpenNETCF.IO.Serial.GPS
{

	public class Movement
	{
		#region Initialization
		int nbspeedvalues=0;
		decimal speedknotssum=0;

		decimal speedknotsaverage=0;
		decimal speedknots=0;
		decimal speedknotsmax=0;

		decimal speedmphaverage=0;
		decimal speedmph=0;
		decimal speedmphmax=0;
		
		decimal speedkphaverage=0;
		decimal speedkph=0;
		decimal speedkphmax=0;
		
		decimal track=0;
		
		//decimal magneticvariation=0;
		decimal magneticvariation;
		
		//CardinalDirection directionmagnetic=CardinalDirection.West;
		CardinalDirection directionmagnetic;

		#endregion

		#region properties
		public int NbSpeedValues
		{
			set
			{
				nbspeedvalues=value;
			}
			get
			{
				return nbspeedvalues;
			}
		}


		public decimal MagneticVariation
		{
			get
			{
				return magneticvariation;
			}
			set
			{
				magneticvariation=value;
			}
		}

		public CardinalDirection DirectionMagnetic
		{
			get
			{
				return directionmagnetic;
			}
			set
			{
				directionmagnetic=value;
			}
		}


		public decimal SpeedKnotsAverage
		{
			get
			{
				return speedknotsaverage;
			}
		}
		public decimal SpeedKnotsMax
		{
			get
			{
					return speedknotsmax;
			}
		}
		public decimal SpeedKnots
		{
			get
			{
				return speedknots;
			}
			set
			{
				speedknots=Math.Round(value,3);
				speedknotssum=speedknotssum+speedknots;
				if (speedknots > speedknotsmax)
				{
					speedknotsmax=Math.Round(speedknots,3);
					speedmphmax=Math.Round(Misc.KnotsToMph(speedknots),3);
					speedkphmax=Math.Round(Misc.KnotsToKph(speedknots),3);
				}
				speedmph=Math.Round(Misc.KnotsToMph(speedknots),3);
				speedkph=Math.Round(Misc.KnotsToKph(speedknots),3);

				if (nbspeedvalues>0)
				{
					speedknotsaverage=Math.Round(speedknotssum/nbspeedvalues,3);
					speedmphaverage=Math.Round(Misc.KnotsToMph(speedknotsaverage),3);
					speedkphaverage=Math.Round(Misc.KnotsToKph(speedknotsaverage),3);
				}
			}

		}


		public decimal SpeedMphAverage
		{
			get
			{
				return speedmphaverage;
			}
		}
		public decimal SpeedMphMax
		{
			get
			{
				return speedmphmax;
			}
		}
		public decimal SpeedMph
		{
			get
			{
				return speedmph;
			}

		}


		public decimal SpeedKphAverage
		{
			get
			{
				return speedkphaverage;
			}
		}
		public decimal SpeedKphMax
		{
			get
			{
				return speedkphmax;
			}
		}
		public decimal SpeedKph
		{
			get
			{
				return speedkph;
			}
		}


		public decimal Track
		{
			get
			{
				return Math.Round(track,1);
			}
			set
			{
				track=value;
			}
		}
		#endregion
	}
}
