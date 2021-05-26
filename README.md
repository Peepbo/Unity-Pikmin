# Unity-Pikmin
'Pikmin'게임을 모작으로 한 Unity3D 개인 프로젝트 입니다.

## 게임 장르
* AI 액션
## 제작 기간
* 60일
## 사용 툴
* Unity3D (2020.1.11f1)

## 주요 기능
#### 1. Class Diagram
* interface와 abstract를 활용하여 상속을 사용해보았다.
* IObject, IFloat, IInteractionObject 등등 interface를 적극적으로 사용하여 다형성을 극대화 시켰다.

#### 2. Utils
* 부동소수점 반올림 오차를 해결하기 위해 거의 같음을 확인하는 AlmostSame 함수를 구현하였다.
* 반올림 운동 공식을 유틸 스크립트에 구현하여 필요한 스크립트에서 받아서 쓸 수 있도록 구현하였다.

#### 3. IInteractionObject
* 공격, 이동과 같은 다양한 기능들을 구현해야 하는 interface 이다.
* 다형성을 위해 interface로 구현하였다.

#### 4. AnimSetting
* 다양한 스크립트에서 한 스크립트를 사용하여 다양한 함수를 호출할 수 있도록 만든 스크립트이다.
* Dictionary와 Action을 사용하여 구현하였다.

#### 5. Object Pool
* 기존에 사용하던 오브젝트 풀의 문제점을 개선하고자 다양한 해결법을 찾았다.
* 오브젝트 추가 및 수정이 불편하여 오브젝트 타입 별로 풀을 따로 만들어 문제점을 개선하였다.
* 물체를 빌리는 방식을 Dictionary와 Parent pool을 사용하여 일정한 속도(O(1))를 갖도록 개선하였다.
