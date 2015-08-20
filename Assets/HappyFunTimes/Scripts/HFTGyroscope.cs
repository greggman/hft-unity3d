/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using UnityEngine;

public class HFTGyroscope {

    // Returns the attitude (ie, orientation in space) of the device.
    public Quaternion attitude
    {
        get { return m_attitude; }
    }

    // Sets or retrieves the enabled status of this gyroscope.
    public bool enabled = true;
    // Returns the gravity acceleration vector expressed in the device's reference frame.
    public Vector3 gravity = new Vector3(0, -1, 0);
    // Returns rotation rate as measured by the device's gyroscope.
    public Vector3 rotationRate = new Vector3();
    // Returns unbiased rotation rate as measured by the device's gyroscope.
    public Vector3 rotationRateUnbiased = new Vector3();
    // Sets or retrieves gyroscope interval in seconds.
    public float updateInterval = 1.0f / 50.0f;
    // Returns the acceleration that the user is giving to the device.
    public Vector3 userAcceleration = new Vector3();

    enum Order {
        XYZ,
        XZY,
        YXZ,
        YZX,
        ZXY,
        ZYX,
    };

    private Quaternion QuatFromEuler(float rx, float ry, float rz, Order order)
    {
        float c1 = Mathf.Cos( rx / 2.0f );
        float c2 = Mathf.Cos( ry / 2.0f );
        float c3 = Mathf.Cos( rz / 2.0f );
        float s1 = Mathf.Sin( rx / 2.0f );
        float s2 = Mathf.Sin( ry / 2.0f );
        float s3 = Mathf.Sin( rz / 2.0f );

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        float w = 0.0f;

        switch ( order ) {
            case Order.XYZ:
                x = s1 * c2 * c3 + c1 * s2 * s3;
                y = c1 * s2 * c3 - s1 * c2 * s3;
                z = c1 * c2 * s3 + s1 * s2 * c3;
                w = c1 * c2 * c3 - s1 * s2 * s3;
                break;

            case Order.YXZ:

                x = s1 * c2 * c3 + c1 * s2 * s3;
                y = c1 * s2 * c3 - s1 * c2 * s3;
                z = c1 * c2 * s3 - s1 * s2 * c3;
                w = c1 * c2 * c3 + s1 * s2 * s3;
                break;

            case Order.ZXY:

                x = s1 * c2 * c3 - c1 * s2 * s3;
                y = c1 * s2 * c3 + s1 * c2 * s3;
                z = c1 * c2 * s3 + s1 * s2 * c3;
                w = c1 * c2 * c3 - s1 * s2 * s3;
                break;

            case Order.ZYX:

                x = s1 * c2 * c3 - c1 * s2 * s3;
                y = c1 * s2 * c3 + s1 * c2 * s3;
                z = c1 * c2 * s3 - s1 * s2 * c3;
                w = c1 * c2 * c3 + s1 * s2 * s3;
                break;

            case Order.YZX:

                x = s1 * c2 * c3 + c1 * s2 * s3;
                y = c1 * s2 * c3 + s1 * c2 * s3;
                z = c1 * c2 * s3 - s1 * s2 * c3;
                w = c1 * c2 * c3 - s1 * s2 * s3;
                break;

            case Order.XZY:

                x = s1 * c2 * c3 - c1 * s2 * s3;
                y = c1 * s2 * c3 - s1 * c2 * s3;
                z = c1 * c2 * s3 + s1 * s2 * c3;
                w = c1 * c2 * c3 + s1 * s2 * s3;
                break;

        }
        return new Quaternion(x, y, z, w);
    }

    public void SetAttitude(float alpha, float beta, float gamma, float orient)
    {
        // var zee = new THREE.Vector3( 0, 0, 1 );
        // var euler = new THREE.Euler();
        // var q0 = new THREE.Quaternion();
        // var q1 = new THREE.Quaternion( - Math.sqrt( 0.5 ), 0, 0, Math.sqrt( 0.5 ) ); // - PI/2 around the x-axis

        alpha = alpha * -Mathf.PI / 180.0f; // Z
        beta  = beta  *  Mathf.PI / 180.0f; // X'
        gamma = gamma *  Mathf.PI / 180.0f; // Y''

        // euler.set( beta, alpha, - gamma, 'YXZ' );                       // 'ZXY' for the device, but 'YXZ' for us
        // quaternion.setFromEuler( euler );                               // orient the device
        Quaternion q = QuatFromEuler( beta, alpha, gamma, Order.YXZ);
        // quaternion.multiply( q1 );                                      // camera looks out the back of the device, not the top
        q = q * m_negativeHalfPiX;
        // quaternion.multiply( q0.setFromAxisAngle( zee, - orient ) );    // adjust for screen orientation
     //   q = q * Quaternion.AngleAxis(-orient, Vector3.forward);

        m_attitude = q;
    }

    private static Quaternion m_negativeHalfPiX = new Quaternion( - Mathf.Sqrt( 0.5f ), 0.0f, 0.0f, Mathf.Sqrt( 0.5f ) );
    private Quaternion m_attitude = new Quaternion();
};
