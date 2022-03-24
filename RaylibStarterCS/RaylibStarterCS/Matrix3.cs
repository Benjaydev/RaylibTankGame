using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{

    public class Matrix3
    {
        // Initialise the matrix values

        public float m00, m10, m20;
        public float m01, m11, m21;
        public float m02, m12, m22;

        public Matrix3()
        {
            m00 = m11 = m22 = m01 = m02 = m10 = m12 = m20 = m21 = 0;
        }

        // Construct matrix with single value
        public Matrix3(float m)
        {
            // Set the main diagonal to the value
            m00 = m11 = m22 = m;
            // Set the rest of the values to 0
            m01 = m02 = m10 = m12 = m20 = m21 = 0;

        }
        public Matrix3(float M00, float M01, float M02, float M10, float M11, float M12, float M20, float M21, float M22)
        {
            // Set each value in the matrix
            m00 = M00; m10 = M10; m20 = M20;
            m01 = M01; m11 = M11; m21 = M21;
            m02 = M02; m12 = M12; m22 = M22;
        }

        // Set each value in the matrix
        public void Set(float M00, float M01, float M02, float M10, float M11, float M12, float M20, float M21, float M22)
        {
            m00 = M00; m10 = M10; m20 = M20;
            m01 = M01; m11 = M11; m21 = M21;
            m02 = M02; m12 = M12; m22 = M22;
        }
        // Set matrix values to another matrix
        public void Set(Matrix3 m)
        {
            m00 = m.m00; m10 = m.m10; m20 = m.m20;
            m01 = m.m01; m11 = m.m11; m21 = m.m21;
            m02 = m.m02; m12 = m.m12; m22 = m.m22;
        }

        // Transpose this matrix
        public void Transpose()
        {
            Matrix3 temp = new Matrix3();
            temp.m00 = m00; temp.m10 = m01; temp.m20 = m02;
            temp.m01 = m10; temp.m11 = m11; temp.m21 = m12;
            temp.m02 = m20; temp.m12 = m21; temp.m22 = m22;

            Set(temp);
        }

        // Get specific row of the matrix, this allows for cleaner code when trying to access individual rows
        public Vector3 GetRow(int row)
        {
            if(row == 0)
            {
                return new Vector3(m00, m10, m20);
            }
            else if(row == 1)
            {
                return new Vector3(m01, m11, m21);
            }
            else if(row == 2)
            {
                return new Vector3(m02, m12, m22);
            }
            return new Vector3(0, 0, 0);
        }


        // Get specific column of the matrix, this allows for cleaner code when trying to access individual columns
        public Vector3 GetColumn(int col)
        {
            if(col == 0)
            {
                return new Vector3(m00, m01, m02);
            }
            else if(col == 1)
            {
                return new Vector3(m10, m11, m12);
            }
            else if(col == 2)
            {
                return new Vector3(m20, m21, m22);
            }
            return new Vector3(0, 0, 0);
        }

        // Set rotation of matrix (This will replace all values already in matrix)
        // Set rotation of X
        public void SetRotateX(double rad)
        {
            Set(1, 0, 0, 0, (float)Math.Cos(rad), (float)Math.Sin(rad), 0, -(float)Math.Sin(rad), (float)Math.Cos(rad));
        }
        // Set rotation of Y
        public void SetRotateY(double rad)
        {
            Set((float)Math.Cos(rad), 0, -(float)Math.Sin(rad), 0, 1, 0, (float)Math.Sin(rad), 0, (float)Math.Cos(rad));
        }
        // Set rotation of Z
        public void SetRotateZ(double rad)
        {
            Set((float)Math.Cos(rad), (float)Math.Sin(rad), 0, -(float)Math.Sin(rad), (float)Math.Cos(rad), 0, 0, 0, 1);
        }

        // Rotate the z axis of matrix by radians
        public void RotateZ(double radZ)
        {
            // Make new matrix for each axis
            Matrix3 z = new Matrix3();
            // Set rotate for each new matrix
            z.SetRotateZ(radZ);

            // Apply rotations to this matrix by multiplying it by each axis matrix and setting
            Set(this * z);

        }

        // Rotate all axes
        public void Rotate(double radX, double radY, double radZ)
        {
            // Make new matrix for each axis
            Matrix3 x = new Matrix3();
            Matrix3 y = new Matrix3();
            Matrix3 z = new Matrix3();
            // Set rotate for each new matrix
            x.SetRotateX(radX);
            y.SetRotateY(radY);
            z.SetRotateZ(radZ);

            // Apply rotations to this matrix by multiplying it by each axis matrix then setting it
            Set(this * z);
            Set(this * y);
            Set(this * x);

        }

        // Rotate multiple axes at once
        public void SetRotate(float pitchX, float yawY, float rollZ)
        {
            // Make new matrix for each axis
            Matrix3 x = new Matrix3();
            Matrix3 y = new Matrix3();
            Matrix3 z = new Matrix3();

            // Set rotate for each value
            x.SetRotateX(pitchX);
            y.SetRotateY(yawY);
            z.SetRotateZ(rollZ);

            // Combine the rotations
            Set(z * y * x);
        }
        // Set scale of matrix
        public void SetScaled(float x, float y, float z)
        {
            m00 = x; m01 = 0; m02 = 0;
            m10 = 0; m11 = y; m12 = 0;
            m20 = 0; m21 = 0; m22 = z;
        }
        public void Scale(float x, float y, float z)
        {
            Matrix3 m = new Matrix3();
            m.SetScaled(x, y, z);

            Set(this * m);
        }


        public void Translate(float x, float y)
        {
            m20 = m20 + x; m21 = m21 + y;
        }

        // Set translation
        public void SetTranslation(float x, float y)
        {
            m20 = x; m21 = y;
        }

      
        // Overload Matrix multiplied by Vector operator (Vector transformation)
        public static Vector3 operator *(Matrix3 M, Vector3 v)
        {
            return new Vector3(
                M.GetRow(0).Dot(v),
                M.GetRow(1).Dot(v),
                M.GetRow(2).Dot(v)
                );
        }

        // Overload Matrix multiplied by Matrix operator (Concatenation) 
        public static Matrix3 operator *(Matrix3 M1, Matrix3 M2)
        {
            return new Matrix3(
                M1.GetRow(0).Dot(M2.GetColumn(0)),
                M1.GetRow(1).Dot(M2.GetColumn(0)),
                M1.GetRow(2).Dot(M2.GetColumn(0)),

                M1.GetRow(0).Dot(M2.GetColumn(1)),
                M1.GetRow(1).Dot(M2.GetColumn(1)),
                M1.GetRow(2).Dot(M2.GetColumn(1)),

                M1.GetRow(0).Dot(M2.GetColumn(2)),
                M1.GetRow(1).Dot(M2.GetColumn(2)),
                M1.GetRow(2).Dot(M2.GetColumn(2)));
        }

        public static Matrix3 operator +(Matrix3 M1, Vector3 v)
        {
            return new Matrix3(M1.m00 + v.x, M1.m01 + v.x, M1.m02 + v.x, M1.m10 + v.y, M1.m11 + v.y, M1.m12 + v.y, M1.m20 + v.z, M1.m21 + v.z, M1.m22 + v.z);
        }


    }
}