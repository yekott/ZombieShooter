# 프로젝트명
좀비슈터

## 주요 코드 설명

  - Scripts/
    - AI/
      - EnemyController.cs (좀비(적 유닛) 컨트롤러 소스코드)
    - CityBuilder/
      - ScriptableObject/
        - Building.cs (건물 Prefab과 해당 건물의 기본 자료)
        - BuildingList.cs (Building.cs 를 담고 있는 스크립터블 오브젝트)
        - BuildingContanier.cs (Container를 담고있는 Building에서 실행시킬 모노스크립트)
        - Container.cs (LootableItem 을 담으려고 했던 스크립트)
        - LootableItem.cs (루팅 가능한 아이템을 모으려고 했던 스크립터블 오브젝트) # 미구현
        - Plains.cs (도시를 지을 environment Prefab을 모아둔 스크립터블 오브젝트)
      - Simulator/
        - CityGenerator.cs 
        - WeatherCyCle.cs
    - Controller/
      - AnimatorHook.cs (ik animation, 무기마다 고정된 손 위치를 추적하기 위해 만든 스크립트)
      - CameraController.cs (Camera를 선택된 메인 캐릭터를 부드럽게 따라다니도록 만든 스크립트)
      - Controller.cs (메인캐릭터 animator, rotate, position, weapon 조작 해주는 스크립트)
      - InputManager.cs (inputmanager)
      - Interfaces.cs (현재는 맞았을때 처리할 OnHit 밖에 없지만 interface를 통해서 스크립트마다 특징을 만들어 줄 수 있음.)
      - LevelObject.cs (OnHit이 제대로 설정되는지 확인한 더미용 스크립트)
      - PlayerMenu.cs (플레이어의 key입력을 받아서 ui 호출하거나 꺼주는 스크립트)
    - Item/
      - Ammo.cs (각 총별로 저장될 데이터를 모아둔 스크립터블 오브젝트)
      - Bullets.cs (발사된 총알의 스크립트(여기서 총알의 방향, 위치를 정해줌.), 데미지나 속도는 Ammo에서 받아옴)
      - Item.cs (모든 아이템의 부모 스크립터블 오브젝트)
      - MagazineHook.cs (현재 해당 탄창안에 들어가있는 Ammo의 종류를 스택으로 받아둠.)
      - Weapon.cs (각 총별로 저장될 데이터를 모아둔 스크립터블 오브젝트)
      - WeaponHook.cs (Weapon을 가지고 데이터를 주고 받을 스크립트)
    - ObjectPool/
      - ObjectPool.cs (instantiate, Delete 가 너무 빈번해지면 비용이 너무 높아지기 때문에 SETACTIVE로 껏다 켰다 하면서 오브젝트를 나누어줄 오브젝트 풀입니다.)

## 게임 화면 스크린샷
### 야간전
![ZombieShooterNight](https://user-images.githubusercontent.com/50022423/106046517-b4352c80-6125-11eb-87dc-cb65b9d31e35.gif)

### 주간전
![ZombieShooterNoon](https://user-images.githubusercontent.com/50022423/106047870-79cc8f00-6127-11eb-8379-dda772ed6b77.gif)

### 스텐실 & 집 투명화
![ZombieShooterStencil](https://user-images.githubusercontent.com/50022423/106048894-ca90b780-6128-11eb-91e7-8b6be4825f66.gif)

### 랜덤거리 
![ZombieShooterRandomMap](https://user-images.githubusercontent.com/50022423/106049648-c2854780-6129-11eb-8ab7-be82a4704cda.gif)
