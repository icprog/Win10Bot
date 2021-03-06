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

using slg.RobotAbstraction.Sensors;
using slg.LibMapping;

namespace slg.RobotBase.Interfaces
{
    public interface IDrive
    {
        IDriveInputs driveInputs { get; set; }

        IOdometry hardwareBrickOdometry { get; set; }       // can be null, then software calculation is used

        bool Enabled { get; set; }

        void Init();

        void Close();

        /// <summary>
        /// apply driveInputs to drive - command the motors.
        /// </summary>
        void Drive();

        /// <summary>
        /// try put drive in a safe position - stop motors etc.
        /// </summary>
        void Stop();

        /// <summary>
        /// resets odometry algorithm in case of invalid wheel ticks changes
        /// </summary>
        void OdometryReset();

        /// <summary>
        /// calculates robot pose change based on current wheel encoders ticks
        /// </summary>
        /// <param name="robotPose">will be adjusted based on wheels travel</param>
        /// <param name="encoderTicks">wheel encoder ticks - left, right...</param>
        /// <returns>Displacement - to be applied in SLAM module</returns>
        IDisplacement OdometryCompute(IRobotPose robotPose, long[] encoderTicks);
    }
}
