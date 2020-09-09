using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections;

namespace Dodger
{
    public class Obstacle
    {
        #region Секция характеристик препятствий

        private const int NUMBER_MESH_TYPES = 5;
        private const float OBJECT_LENGTH = 3.0f;
        private const float OBJECT_RADIUS = OBJECT_LENGTH / 2.0f;
        private const int OBJECT_STACKS_SLICES = 18;

        // Цвета препятствий
        private static readonly Color[] ObstacleColors =
      {
        Color.Red, Color.Blue, Color.Green,
        Color.Bisque, Color.Cyan, Color.DarkKhaki,
        Color.OldLace, Color.PowderBlue, Color.DarkTurquoise,
        Color.Azure, Color.Violet, Color.Tomato,
        Color.Yellow, Color.Purple, Color.AliceBlue,
        Color.Honeydew, Color.Crimson, Color.Firebrick
      };

        // Ссылки для Mesh-информации
        private Mesh obstacleMesh = null;
        private Material obstacleMaterial;
        private Vector3 position;
        private bool isTeapot;

        // Переменные для вращения препятствий
        private float rotation = 0;
        private float rotationspeed = 0.0f;
        private Vector3 rotationVector;
        #endregion

        // Параметризованный конструктор
        public Obstacle(Device device, float x, float y, float z)
        {
            // Сохраняем нашу позицию
            position = new Vector3(x, y, z);

            // Пока это - не заварочный чайник
            isTeapot = false;

            // Создать новое препятствие
            switch (Utility.Rnd.Next(NUMBER_MESH_TYPES))
            {
                case 0:
                    obstacleMesh = Mesh.Sphere(device,
                        OBJECT_RADIUS,
                        OBJECT_STACKS_SLICES,
                        OBJECT_STACKS_SLICES);
                    break;
                case 1:
                    obstacleMesh = Mesh.Box(device,
                        OBJECT_LENGTH,
                        OBJECT_LENGTH,
                        OBJECT_LENGTH);
                    break;
                case 2:
                    obstacleMesh = Mesh.Teapot(device);
                    isTeapot = true;
                    break;
                case 3:
                    obstacleMesh = Mesh.Cylinder(device,
                        OBJECT_RADIUS,
                        OBJECT_RADIUS,
                        OBJECT_LENGTH,
                        OBJECT_STACKS_SLICES,
                        OBJECT_STACKS_SLICES);
                    break;
                case 4:
                    obstacleMesh = Mesh.Torus(device,
                        OBJECT_RADIUS / 3.0f,
                        OBJECT_RADIUS / 2.0f,
                        OBJECT_STACKS_SLICES,
                        OBJECT_STACKS_SLICES);
                    break;
            }

            // Установить цвет препятствия
            obstacleMaterial = new Material();
            Color objColor = ObstacleColors[Utility.Rnd.Next(ObstacleColors.Length)];
            obstacleMaterial.Ambient = objColor;
            obstacleMaterial.Diffuse = objColor;

            // Параметры вращения препятствий
            rotationspeed = (float)Utility.Rnd.NextDouble() * (float)Math.PI;
            rotationVector = new Vector3((float)Utility.Rnd.NextDouble(),
                (float)Utility.Rnd.NextDouble(), (float)Utility.Rnd.NextDouble());
        }

        // Вычисление положения препятствия
        public void LocationObstacle(float elapsedTime, float speed)
        {
            position.Z += (speed * elapsedTime);
            rotation += rotationspeed * elapsedTime;

        }

        // Свойство глубины класса Obstacle
        public float Depth
        {
            get { return position.Z; }
        }

        // Рисование препятствия
        public void DrawObstacle(Device device)
        {
            if (isTeapot)
            {
                device.Transform.World =
                    Matrix.RotationAxis(rotationVector, rotation) *
                    Matrix.Scaling(OBJECT_RADIUS, OBJECT_RADIUS, OBJECT_RADIUS) *
                    Matrix.Translation(position);
            }
            else
            {
                device.Transform.World =
                    Matrix.RotationAxis(rotationVector, rotation) *
                    Matrix.Translation(position);
            }

            device.Material = obstacleMaterial;
            device.SetTexture(0, null);
            obstacleMesh.DrawSubset(0);

        }

        // Обнаружение столкновений автомобиля с препятствием
        public bool IsHittingCar(float carLocation, float carDiameter)
        {
            if (position.Z > (Car.DEPTH - (carDiameter / 2.0f)))
            {
                // Проверка столкновения на правой стороне дороги
                if ((carLocation < 0) && (position.X < 0))
                    return true;

                // Проверка столкновения на левой стороне дороги
                if ((carLocation > 0) && (position.X > 0))
                    return true;
            }
            return false;
        }



        #region Освобождение ресурсов для сборщика мусора

        // Для принудительного вызова при смене препятствий
        public void Dispose()
        {
            obstacleMesh.Dispose();
            System.GC.SuppressFinalize(this);
        }

        // Для автоматического вызова при завершении игры
        ~Obstacle() // Деструктор
        {
            this.Dispose();
        }

        #endregion

    }

    // Класс-коллекция для хранения данных о препятствиях
    public class Obstacles : IEnumerable
    {
        // Создать объект динамического массива 
        // с автоматическим изменением длины
        private ArrayList obstacleList = new ArrayList();

        // Создание свойства возможности адресации 
        // элементов коллекции по индексу
        public Obstacle this[int index]
        {
            get
            {
                return (Obstacle)obstacleList[index];
            }
        }

        // Переопределение интерфейсного метода сброса курсора 
        // нумератора в начало динамического массива 
        public IEnumerator GetEnumerator()
        {
            return obstacleList.GetEnumerator();
        }

        // Добавление препятствия в динамический массив
        public void Add(Obstacle obstacle)
        {
            obstacleList.Add(obstacle);
        }

        // Удаление препятствия из динамического массива
        public void Remove(Obstacle obstacle)
        {
            obstacleList.Remove(obstacle);
        }

        // Очистка коллекции препятствий
        public void Clear()
        {
            obstacleList.Clear();
        }
    }

}
