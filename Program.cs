using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Dodger
{
    class Car
    {
        #region Секция переменных-характеристик автомобиля
        // Константы
        public const float HEIGHT = 2.5f;// Высота автомобиля
        public const float DEPTH = 3.0f;// Глубина автомобиля
        public const float SPEED_INCREMENT = 0.1f;// Приращение скорости бокового перемещения
        private const float SCALE = 0.85f; // Отношение размера автомобиля к ширине дороги

        // Переменные
        private float carLocation = DodgerGame.ROAD_LOCATION_LEFT;// Текущее положение слева
        private float carDiameter;// Диаметр для расчета столкновений с препятствием
        private float carSpeed = 10.0f;// Текущая боковая скорость автомобиля
        private bool movingLeft = false;// Направление перемещения влево
        private bool movingRight = false;// Направление перемещения вправо

        // Ссылки для Mesh-объекта автомобиля
        private Mesh carMesh = null;
        private Material[] carMaterials = null;
        private Texture[] carTextures = null;

        // Общедоступные свойства для характеристик класса Car
        public float Location // Текущее положение слева
        {
            get { return carLocation; }
            set { carLocation = value; }
        }

        public float Diameter // Диаметр для расчета столкновений с препятствием
        {
            get { return carDiameter; }
        }

        public float Speed // Текущая боковая скорость автомобиля
        {
            get { return carSpeed; }
            set { carSpeed = value; }
        }

        public bool IsMovingLeft // Направление перемещения влево
        {
            get { return movingLeft; }
            set { movingLeft = value; }
        }

        public bool IsMovingRight // Направление перемещения вправо
        {
            get { return movingRight; }
            set { movingRight = value; }
        }

        #endregion

        // Параметризованный конструктор класса
        public Car(Device device)
        {
            // Создать и загрузить Mesh-объект "Автомобиль",
            // используя ранее разработанный метод класса DodgerGame
            carMesh = DodgerGame.LoadMesh(device, @".\car.x",
                ref carMaterials, ref carTextures);

            // Мы должны вычислить сферу ограничения для нашего автомобиля
            VertexBuffer vb = carMesh.VertexBuffer;
            try
            {
                // Замкнуть вершинный буфер перед вычислением
                GraphicsStream stm = vb.Lock(0, 0, LockFlags.None);
                Vector3 center; // Мы не будем использовать центр, 
                //но ссылка на него требуется
                float radius = Geometry.ComputeBoundingSphere(stm,
                    carMesh.NumberVertices, carMesh.VertexFormat, out center);

                // Вычислим диаметр автомобиля
                carDiameter = (radius * 2) * SCALE;
            }
            finally
            {
                // Независимо от результата попытки
                // отмыкаем и устанавливаем вершинный буфер
                vb.Unlock();
                vb.Dispose();
            }
        }

        // Функция прорисовки автомобиля
        public void DrawCar(Device device)
        {
            // Автомобиль является слишком большим, предварительно уменьшим его
            device.Transform.World = Matrix.Scaling(SCALE, SCALE, SCALE)
                * Matrix.Translation(carLocation, HEIGHT, DEPTH);

            for (int i = 0; i < carMaterials.Length; i++)
            {
                device.Material = carMaterials[i];
                device.SetTexture(0, carTextures[i]);
                carMesh.DrawSubset(i);
            }
        }

        // Управление перемещением автомобиля
        public void LocationCar(float elapsedTime)
        {
            // Перемещаем влево
            if (movingLeft)
            {
                this.Location += carSpeed * elapsedTime;

                // Если достигли левого края дороги...
                if (this.Location >= DodgerGame.ROAD_LOCATION_LEFT)
                {
                    movingLeft = false;
                    this.Location = DodgerGame.ROAD_LOCATION_LEFT;
                }
            }

            // Перемещаем вправо
            if (movingRight)
            {
                this.Location -= carSpeed * elapsedTime;

                // Если достигли правого края дороги...
                if (this.Location <= DodgerGame.ROAD_LOCATION_RIGHT)
                {
                    movingRight = false;
                    this.Location = DodgerGame.ROAD_LOCATION_RIGHT;
                }
            }
        }

        public void IncrementSpeed()
        {
            carSpeed += SPEED_INCREMENT;
        }

    }
}