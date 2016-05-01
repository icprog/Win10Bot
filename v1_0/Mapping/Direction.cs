﻿/*
 * Copyright (c) 2016..., Sergei Grichine   http://trackroamer.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *    
 * this is a no-warranty no-liability permissive license - you do not have to publish your changes,
 * although doing so, donating and contributing is always appreciated
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using slg.RobotMath;

namespace slg.Mapping
{
    public interface IDirection : ICloneable
    {
        // everything in degrees
        double? heading { get; set; }
        double? bearing { get; set; }
        double? distanceToGoalMeters { get; set; }
        double? course { get; set; }
        double? bearingRelative { get; set; }
        double? turnRelative { get; }
    }

    [DataContract]
    //[Serializable]
    public class Direction : DirectionMath, IDirection
    {
        // see http://answers.yahoo.com/question/index?qid=20081117160002AADh95q
        // see http://www.rvs.uni-bielefeld.de/publications/Incidents/DOCS/Research/Rvs/Misc/Additional/Reports/adf.gif

        /*
            Heading is not always the direction an aircraft is moving. That is called 'course'. Heading is the direction the aircraft is pointing.
            The aircraft may be drifting a little or a lot due to a crosswind.
            Bearing is the angle in degrees (clockwise) between North and the direction to the destination or nav aid.
            Relative bearing is the angle in degrees (clockwise) between the heading of the aircraft and the destination or nav aid.
         */

        /// <summary>
        /// Heading is the direction the robot is pointing, degrees; same as "course" for a ground platform; true North is "0"
        /// </summary>
        public double? heading { get; set; }

        /// <summary>
        /// Bearing is the angle in degrees (clockwise) between North and the direction to the destination or nav aid.
        /// Degrees; usually to target or obstacle; absolute, related to true North
        /// </summary>
        public double? bearing { get; set; }

        /// <summary>
        /// distance to goal, meters; usually to target or obstacle; can be null, but not negative.
        /// </summary>
        public double? distanceToGoalMeters { get; set; }


        public double? course       // degrees, guaranteed to be between 0...360
        {
            get
            {
                return to360(heading);
            }
            set
            {
                heading = value % 360.0d;
                if (heading < 0.0d)
                {
                    heading += 360.0d;
                }
            }
        }

        public double? bearingRelative    // use only if heading is defined
        {
            get
            {
                if (heading.HasValue && bearing.HasValue)
                {
                    // calculate turn is between -180...180:

                    return to180((double)(bearing - heading));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (heading.HasValue && value.HasValue)
                {
                    bearing = to360((double)(heading + value));
                }
                else
                {
                    bearing = null;
                }
            }
        }

        public double? turnRelative
        {
            get
            {
                if (heading.HasValue && bearing.HasValue)
                {
                    // calculate turn is between -180...180:

                    return to180((double)(bearing - heading));
                }
                else
                {
                    return null;
                }
            }
        }

        public long TimeStamp = 0L;

        // for compass related calculations, use GeoPosition::magneticVariation() to offset true North

        public Direction()
        {
        }

        #region ICloneable Members

        public object Clone()
        {
            // Direction ret = new Direction() { bearing = this.bearing, heading = this.heading };
            return this.MemberwiseClone();  // shallow copy, only value types are cloned
        }

        #endregion // ICloneable Members

        public override string ToString()
        {
            return string.Format("({0},{1})", heading, bearing);
        }
    }
}
