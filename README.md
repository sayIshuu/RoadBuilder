# RoadBuilder

> 랜덤으로 제공되는 길블럭으로 길의 끝과 끝을 연결하여 점수를 얻는 캐주얼 퍼즐게임

## 제작 정보
- **제작 인원**: Unity 개발자 4인
- **제작 기간**: 1주일

---

## 게임 진행

| ![게임 진행 1](https://github.com/user-attachments/assets/c27a5adb-8029-4cdd-a0f3-fa72b45c7fbe)| ![게임 진행 2](https://github.com/user-attachments/assets/1cbcaf5d-f97a-43f3-9163-aca0fb8b88a1)| ![게임 진행 3](https://github.com/user-attachments/assets/a3e1a28c-a866-44a1-8c24-0888d901b73d) |
|:--:|:--:|:--:|
| 랜덤으로 주어진 블록 선택 | 블록을 터치 & 드래그하여 이동 | 길을 연결하면 점수 획득 |

1. **화면 슬라이드**를 통해 랜덤으로 주어진 세 개의 블록 중에서 드래그할 블록을 선택할 수 있습니다.
2. **화면 어디든 터치 & 드래그**로 선택된 블록을 옮길 수 있습니다.
3. 배치된 블록들이 만든 하얀색 길이 **격자판의 끝과 끝을 연결**하면 길이 파괴되며 점수를 얻습니다.

---

## 종료 조건

<table>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/fc75b2d6-7a11-4cca-8b59-e493924630bc" width="250"></td>
    <td><img src="https://github.com/user-attachments/assets/34ad5c4d-1986-4415-ab1f-96e28758e41a" width="250"></td>
  </tr>
  <tr>
    <td>블록이 쌓여 더 이상 배치 불가</td>
    <td>목표 점수를 충족하지 못함</td>
    <td></td>
  </tr>
</table>

1. 길을 잇지 못한 블록들이 쌓여 **더 이상 격자판에 블록을 배치할 수 없을 때** 게임이 종료됩니다.
2. **10턴마다 갱신되는 Goal Score** (왼쪽 아래 인터페이스)를 현재 점수로 충족하지 못하면 게임이 종료됩니다.
   - 자칫 루즈해질 수 있는 퍼즐게임에 **중간 목표인 Goal Score 콘텐츠를 추가**하여 전략의 다양성을 높였습니다.

---

## 추가적인 재미 요소

<table>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/77ca4305-a79c-4c92-bde6-2a25376a0b0a" width="250"></td>
    <td><img src="https://github.com/user-attachments/assets/1b1666b2-9039-4c65-a0c3-47b6515e702b" width="250"></td>
  </tr>
  <tr>
    <td>낮은 확률로 삼차선, 사차선 타일 등장</td>
    <td>업적 시스템 및 최고 점수 기록</td>
    <td></td>
  </tr>
</table>

1. 낮은 확률로 **삼차선, 사차선 타일**이 등장하여 전략적 요소를 강화합니다.
2. **업적 콘텐츠 추가**: 한 번에 얼마나 긴 블록을 연결했는지에 대한 업적이 존재하며, 달성 여부와 최고 점수가 기록됩니다.

---

## 점수 계산 방식 & 추가 정보
- 연결한 길의 **길이³ (세제곱)** 에 해당하는 점수를 획득합니다.
- **타일 제공 인터페이스의 왼쪽 위에 "리롤" 기능**이 있어 **턴 소모 없이 새로운 타일을 받을 수 있습니다.**
- 리롤 기능은 **100턴을 넘길 시 1회씩 재충전**됩니다.
- **매 9턴째에 Goal Score를 충족했는지 여부를 인터페이스 색 변화로 알려줍니다.**
