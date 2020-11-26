# Unity-CG-Destroyer
Gyut and Chan's game development project

​    

**Code Convention**
----

**reference_01 :  http://lonpeach.com/2017/12/24/CSharp-Coding-Standard/ **

**reference_02 :  https://overface.tistory.com/9 **

1. 클래스 변수 선언시 변수명 제일 앞에 **'\_'(언더바)**
2. 1) [bool] 지역변수는 변수명 제일 앞에 **'b'**
   2) [bool] 프로퍼티는 **'is'** 사용
   3) [bool] 배열/리스트/액션 리스트 등은 맨 앞에 **'bool'**
3. **private** 멤버 변수 앞에는 **'m_'**
   - 나머지 멤버 변수는 파스칼 케이스
4. **IMG**, **SPT** 등의 유니티 기본 객체들은 줄이고 언더바를 사용하지 않고, **CANVAS**, **POPUP** 등 지정되어 있지 않은 객체들은 **CANVAS\_ **, **POPUP\_** 과 같이 언더바
5. 코루틴용 함수들은 맨 앞에 **'Coroutine'**
6. **non-public** 메서드면 맨 뒤에 **'internal'**
7. 인터페이스 맨 앞에 **'I'**
   - 열거체 맨 앞에 **'E'**
8. **namespace** 시작은 **CG.##**
9. 재귀 함수 맨 앞에 **'Recrusive'**
10. **ASSERT**는 **RELEASE** / **DEBUG** 버전을 구분
11. **bit flag** 열거 형의 이름은 맨 뒤에 **Flags**
12. **생성자**

```c#
explicit Test(int a, char t, float c)
			: a(a), t(t), c(c)
			{

			}
```

13. 상수 맨 앞에 **'k'**
14. 클래스 메소드 내부에서 클래스 멤버 변수 접근은 **this.**

