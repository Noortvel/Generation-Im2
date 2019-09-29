using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoizeGenerator
{
    private float[,] field;
    private int width, height;
    private System.Random random;

    private float power;
    private float noizeHeight;
    private float nozieMiddle;
    private float shapingPower;

    public NoizeGenerator(float[,] field, float power, float shapingPower)
    {
        this.field = field;
        height = field.GetLength(0);
        width = field.GetLength(1);
        this.power = power;
        this.shapingPower = shapingPower;

        random = new System.Random();
        Generate();
    }

    public void Generate()
    {
        CaclField();
        ShapingField();
    }
    private void CaclField()
    {
        noizeHeight = float.MinValue;
        nozieMiddle = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float seed = (float)random.NextDouble();
                float x = i * seed;
                float y = j * seed;
                float val = Mathf.PerlinNoise(x, y) * power;
                field[i, j] = val;
                nozieMiddle += field[i, j];
                if (field[i, j] > noizeHeight)
                {
                    noizeHeight = field[i, j];
                }
            }
        }
        nozieMiddle /= height * width;
        //print("Height " + noizeHeight);
        //print("Middle" + nozieMiddle);
    }
    private void ShapingField()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (field[i, j] >= nozieMiddle)
                {
                    field[i, j] *= shapingPower;
                }
                else
                {
                    field[i, j] /= shapingPower;
                }
            }
        }
    }
}
