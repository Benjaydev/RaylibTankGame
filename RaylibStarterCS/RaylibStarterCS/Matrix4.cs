using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MathClasses
{
    public struct Matrix4
    {
        // Initialise matrix values
        public float m00, m01, m02, m03;
        public float m10, m11, m12, m13;
        public float m20, m21, m22, m23;
        public float m30, m31, m32, m33;

        // Construct matrix with single value
        public Matrix4(float m)
        {
            // Set the main diagonal to the value
            m00 = m11 = m22 = m33 = m;
            // Set the rest of the values to 0
            m01 = m02 = m03 = m10 = m12 = m13 = m20 = m21 = m23 = m30 = m31 = m32 = 0;
        }

        // Construct using individual values
        public Matrix4(float M00, float M01, float M02, float M03, float M10, float M11, float M12, float M13, float M20, float M21, float M22, float M23, float M30, float M31, float M32, float M33)
        {
            // Set each value in the matrix
            m00 = M00; m10 = M10; m20 = M20; m30 = M30; 
            m01 = M01; m11 = M11; m21 = M21; m31 = M31;
            m02 = M02; m12 = M12; m22 = M22; m32 = M32;  
            m03 = M03; m13 = M13; m23 = M23; m33 = M33;
        }

        // Set each value in the matrix
        public void Set(float M00, float M01, float M02, float M03, float M10, float M11, float M12, float M13, float M20, float M21, float M22, float M23, float M30, float M31, float M32, float M33)
        {
            m00 = M00; m10 = M10; m20 = M20; m30 = M30;
            m01 = M01; m11 = M11; m21 = M21; m31 = M31;
            m02 = M02; m12 = M12; m22 = M22; m32 = M32;
            m03 = M03; m13 = M13; m23 = M23; m33 = M33;
        }

        // Get specific row of the matrix, this allows for cleaner code when trying to access individual rows
        public Vector4 GetRow(int row)
        {
            if(row == 0)
            {
                return new Vector4(m00, m10, m20,m30);
            }
            else if(row == 1)
            {
                return new Vector4(m01, m11, m21, m31);
            }
            else if(row == 2)
            {
                return new Vector4(m02, m12, m22, m32);
            }
            else if(row == 3)
            {
                return new Vector4(m03, m13, m23, m33);
            }
            return new Vector4(0, 0, 0, 0);
        }

        // Get specific column of the matrix, this allows for cleaner code when trying to access individual columns
        public Vector4 GetColumn(int col)
        {
            if(col == 0)
            {
                return new Vector4(m00, m01, m02, m03);
            }
            else if(col == 1)
            {
                return new Vector4(m10, m11, m12, m13);
            }
            else if( col == 2)
            {
                return new Vector4(m20, m21, m22, m23);
            }
            else if(col == 3)
            {
                return new Vector4(m30, m31, m32, m33);
            }
            return new Vector4(0, 0, 0, 0);
        }

        // Set rotation of matrix (This will replace all values already in matrix)
        // Set rotation of X
        public void SetRotateX(double rad)
        {
            Set(1, 0, 0, 0, 0, (float)Math.Cos(rad), (float)Math.Sin(rad), 0, 0, -(float)Math.Sin(rad), (float)Math.Cos(rad), 0, 0, 0, 0, 1);
        }
        // Set rotation of Y
        public void SetRotateY(double rad)
        {
            Set((float)Math.Cos(rad), 0, -(float)Math.Sin(rad), 0, 0, 1, 0, 0, (float)Math.Sin(rad), 0, (float)Math.Cos(rad), 0, 0, 0, 0, 1);
        }
        // Set rotation of Z
        public void SetRotateZ(double rad)
        {
            Set((float)Math.Cos(rad), (float)Math.Sin(rad), 0, 0, -(float)Math.Sin(rad), (float)Math.Cos(rad), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        }

        // Overload Matrix multiplied by Vector operator
        public static Vector4 operator *(Matrix4 M, Vector4 v)
        {
            return new Vector4(
                M.GetRow(0).Dot(v),
                M.GetRow(1).Dot(v),
                M.GetRow(2).Dot(v),
                M.GetRow(3).Dot(v)
                );
        }

        // Overload Matrix multiplied by Matrix operator
        public static Matrix4 operator *(Matrix4 M1, Matrix4 M2)
        {
            return new Matrix4(
                M1.GetRow(0).Dot(M2.GetColumn(0)),
                M1.GetRow(1).Dot(M2.GetColumn(0)),
                M1.GetRow(2).Dot(M2.GetColumn(0)),
                M1.GetRow(3).Dot(M2.GetColumn(0)),

                M1.GetRow(0).Dot(M2.GetColumn(1)),
                M1.GetRow(1).Dot(M2.GetColumn(1)),
                M1.GetRow(2).Dot(M2.GetColumn(1)),
                M1.GetRow(3).Dot(M2.GetColumn(1)),

                M1.GetRow(0).Dot(M2.GetColumn(2)),
                M1.GetRow(1).Dot(M2.GetColumn(2)),
                M1.GetRow(2).Dot(M2.GetColumn(2)),
                M1.GetRow(3).Dot(M2.GetColumn(2)),

                M1.GetRow(0).Dot(M2.GetColumn(3)),
                M1.GetRow(1).Dot(M2.GetColumn(3)),
                M1.GetRow(2).Dot(M2.GetColumn(3)),
                M1.GetRow(3).Dot(M2.GetColumn(3))
                );

        }
    }
}
