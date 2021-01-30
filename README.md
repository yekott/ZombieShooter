# 프로젝트명
좀비슈터

## 주요 코드 설명

-Assets/
  - Scripts/
    - AI/
      - EnemyController.cs (좀비(적 유닛) 컨트롤러 소스코드)
    - CityBuilder/
      - ScriptableObject/
        - Building.cs (건물 Prefab과 해당 건물의 기본 자료)
        - BuildingList.cs (Building.cs ScriptableObjects를 담고 있는 ScriptableObject)
        - BuildingContanier.cs (Container를 담고있는 Building에서 실행시킬 MonoBehavior)
        - Container.cs (LootableItem 을 담으려고 했던 Container)
        - LootableItem.cs (루팅 가능한 아이템을 모으려고 했던 코드) # 미구현
        - Plains.cs (도시를 지을 environment Prefab을 모아둔 Scriptable Object)
      - Simulator/
        - CityGenerator.cs 
        - WeatherCyCle.cs
    - Controller/
      - AnimatorHook.cs
      - CameraController.cs
      - Controller.cs
      - InputManager.cs
      - Interfaces.cs
      - LevelObject.cs
      - PlayerMenu.cs
    - Item/
      - Ammo.cs
      - Bullets.cs
      - Item.cs
      - MagazineHook.cs
      - Weapon.cs
      - WeaponHook.cs
    - ObjectPool/
      - ObjectPool.cs

## 게임 화면 스크린샷
### 야간전
![ZombieShooterNight](https://user-images.githubusercontent.com/50022423/106046517-b4352c80-6125-11eb-87dc-cb65b9d31e35.gif)

### 주간전
![ZombieShooterNoon](https://user-images.githubusercontent.com/50022423/106047870-79cc8f00-6127-11eb-8379-dda772ed6b77.gif)

### 스텐실 & 집 투명화
![ZombieShooterStencil](https://user-images.githubusercontent.com/50022423/106048894-ca90b780-6128-11eb-91e7-8b6be4825f66.gif)

### 랜덤거리 
![ZombieShooterRandomMap](https://user-images.githubusercontent.com/50022423/106049648-c2854780-6129-11eb-8ab7-be82a4704cda.gif)
