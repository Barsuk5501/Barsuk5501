using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
public class NewBehaviourScript1 : MonoBehaviour {

	private Renderer rend; // Рендер
	public int pixWidth = 64; // Ширина
	public int pixHeight = 64;
	float j = 0.1f;
	bool Created = true;
	GameObject k;
	public GameObject plane;
	public int x;
	public int y;
	Vector3 thisCord;
	float seed;
	public Texture2D[] Texts = new Texture2D[4];
	public Texture2D Sum;

	void Create (){ 
		GameObject l = (GameObject)Instantiate (plane, new Vector3 (0, 0, 0), Quaternion.identity); // Создание объекта Plane, присовение ему текстуры
		l.transform.eulerAngles = new Vector3 (-90, 0, 0);
		rend = l.GetComponent<Renderer>();
		rend.material.mainTexture = Sum;

	}


	void Start () {
		Texts [0] = new Texture2D(pixWidth,pixWidth); // Новые текстуры
		Texts [1] = new Texture2D(pixWidth,pixWidth);
		Texts [2] = new Texture2D(pixWidth,pixWidth);
		Texts [3] = new Texture2D(pixWidth,pixWidth);


		seed = Random.Range (99999, 999999); // Случайное число.
		seed = seed - 0.5f;

		FirstFourDots (0, 2, 1.3f); // Вызов шумовой функции 
		FirstFourDots (1, 4, 0.5f);
		FirstFourDots (2, 8, 1f);
		FirstFourDots (3, 16, 0.8f );
	}	

	void FirstFourDots (int i, int per, float amp){ // amp - амплитуда, per - период. // i - номер текстуры. Функция назначает высоту первым четырем точкам по углам текстуры
		Texts[i] = new Texture2D (pixWidth, pixWidth);
		Vector3 LD = new Vector3 (0, 0,Mathf.PerlinNoise(0+seed,0+seed)*amp);
		Texts[i].SetPixel ((int)LD.x, (int)LD.y, new Color (LD.z, LD.z, LD.z, 1)*amp);
		Vector3 LU = new Vector3 (0, pixWidth,Mathf.PerlinNoise(0+seed,pixWidth+seed)*amp);
		Texts[i].SetPixel ((int)LU.x, (int)LU.y, new Color (LU.z, LU.z, LU.z, 1));
		Vector3 RU = new Vector3 (pixWidth, pixWidth,Mathf.PerlinNoise(pixWidth+seed,pixWidth+seed)*amp);
		Texts[i].SetPixel ((int)RU.x, (int)RU.y, new Color (RU.z, RU.z, RU.z, 1));
		Vector3 RD = new Vector3 (pixWidth, 0,Mathf.PerlinNoise(pixWidth+seed,0+seed)*amp);
		Texts[i].SetPixel ((int)RD.x, (int)RD.y, new Color (RD.z, RD.z, RD.z, 1));
		Texts[i].Apply ();

		MakeNoise (LD, LU, RU, RD, pixWidth/2, per, i, amp);  //Принимает координаты четырех углов квадрата и его ширину.
	}



	// LD - Левый нижний угол (Left Down), далее по аналогии.
	void MakeNoise (Vector3 LD,Vector3 LU,Vector3 RU,Vector3 RD, int offset, int per, int i, float amp) { //Ищет средние боковые точки и центральную
		Vector3 LM = new Vector3 (LU.x,((LU.y+LD.y)/2), Mathf.PerlinNoise(LU.x+seed,(LU.y+LD.y)/2+seed)*amp); 
		Texts[i].SetPixel ((int)LM.x,(int)LM.y,new Color (LM.z*LM.z,LM.z*LM.z,LM.z*LM.z,1));
		Vector3 UM = new Vector3 ((LU.x+RU.x)/2, LU.y,  Mathf.PerlinNoise((LU.x+RU.x)/2+seed,LU.y+seed)*amp);
		Texts[i].SetPixel ((int)UM.x, (int)UM.y, new Color (UM.z*UM.z, UM.z*UM.z, UM.z*UM.z, 1));
		Vector3 RM = new Vector3 (RU.x, (RU.y+RD.y)/2,  Mathf.PerlinNoise(RU.x+seed,(RU.y+RD.y)/2+seed)*amp);
		Texts[i].SetPixel ((int)RM.x, (int)RM.y, new Color (RM.z*RM.z, RM.z*RM.z, RM.z*RM.z, 1));
		Vector3 DM = new Vector3 ((LD.x+RD.x)/2,LD.y,   Mathf.PerlinNoise ((LD.x+RD.x)/2+seed, LD.y+seed)*amp);
		Texts[i].SetPixel ((int)DM.x, (int)DM.y, new Color (DM.z*DM.z, DM.z*DM.z, DM.z*DM.z, 1));
		Vector3 MM = new Vector3 ((LD.x+RD.x)/2,(RU.y+RD.y)/2, Mathf.PerlinNoise ((LD.x+RD.x)/2+seed, (RU.y+RD.y)/2)*amp);
		Texts[i].SetPixel ((int)MM.x, (int)MM.y, new Color (MM.z*MM.z, MM.z*MM.z, MM.z*MM.z, 1));

		Texts[i].Apply (); // Применить

		if (offset >= per) {
			MakeNoise (LM, LU, UM, MM, offset/2, per, i, amp); 
			MakeNoise (MM, UM, RU, RM, offset/2, per, i, amp);
			MakeNoise (LD, LM, MM, DM, offset/2, per, i, amp);
			MakeNoise (DM, MM, RM, RD, offset/2, per, i, amp);
		}
		if (offset < 2) {
			offset = 2;
		}

		if (offset < per) { // Функция сглаживания
			lerp(LM, LU, UM, MM, offset/2, i, amp);
			lerp (MM, UM, RU, RM, offset/2,  i, amp);
			lerp (LD, LM, MM, DM, offset/2,  i, amp);
			lerp (DM, MM, RM, RD, offset/2,  i, amp);
		}
		}


	void lerp (Vector3 LD,Vector3 LU,Vector3 RU,Vector3 RD, int offset, int i, float amp){
		Vector3 LM = new Vector3 (LU.x,((LU.y+LD.y)/2), (LU.z+LD.z)/2*amp);
		Texts[i].SetPixel ((int)LM.x,(int)LM.y,new Color (LM.z*LM.z,LM.z*LM.z,LM.z*LM.z,1));
		Vector3 UM = new Vector3 ((LU.x+RU.x)/2, LU.y,  (LU.z+RU.z)/2*amp);
		Texts[i].SetPixel ((int)UM.x, (int)UM.y, new Color (UM.z*UM.z, UM.z*UM.z, UM.z*UM.z, 1));
		Vector3 RM = new Vector3 (RU.x, (RU.y+RD.y)/2,  (RU.z+RD.z)/2*amp);
		Texts[i].SetPixel ((int)RM.x, (int)RM.y, new Color (RM.z*RM.z, RM.z*RM.z, RM.z*RM.z, 1));
		Vector3 DM = new Vector3 ((LD.x+RD.x)/2,LD.y, (RD.z+LD.z)/2*amp);
		Texts[i].SetPixel ((int)DM.x, (int)DM.y, new Color (DM.z*DM.z, DM.z*DM.z, DM.z*DM.z, 1));
		Vector3 MM = new Vector3 ((LD.x+RD.x)/2,(RU.y+RD.y)/2, (LD.z+LU.z+RU.z+RD.z)/4*amp);
		Texts[i].SetPixel ((int)MM.x, (int)MM.y, new Color (MM.z*MM.z, MM.z*MM.z, MM.z*MM.z, 1));
		
		Texts[i].Apply ();

		if (offset >= 2) {
			lerp(LM, LU, UM, MM, offset/2, i, amp);
			lerp (MM, UM, RU, RM, offset/2, i, amp);
			lerp (LD, LM, MM, DM, offset/2, i, amp);
			lerp (DM, MM, RM, RD, offset/2, i, amp);
		}
	} 


	void WaterLine () // Все пиксели выше 0.5 белые, остальные синие.
	{

			for (int x = 0; x<=pixWidth; x++) {
				for (int y = 0; y<=pixWidth; y++) {
					Color pixx = Sum.GetPixel (x, y);
					float poxx = pixx.b;
					if (poxx > 0.5f)
						Sum.SetPixel (x, y, Color.white);
					if (poxx <= 0.5f)
						Sum.SetPixel (x, y, Color.blue);
					
					if (x == pixWidth && y == pixHeight) {
						Created = true;
						Sum.Apply ();
					}
				}
			}
	}

	void Update () {
	
		if (Input.GetKeyDown (KeyCode.Space)) {
			Sum = new Texture2D(pixWidth,pixWidth); // Прибавить все значения текстур в одну.
			for (int x = 0; x<pixWidth; x++){
				for (int y = 0; y<pixWidth; y++){
					float h1 = Texts[0].GetPixel(x,y).b;
					float h2 = Texts[1].GetPixel(x,y).b;
					float h3 = Texts[2].GetPixel(x,y).b;
					float h4 = Texts[3].GetPixel(x,y).b;
					float h5 = h1+h2+h3+h4;
					Sum.SetPixel(x,y,new Color(h5,h5,h5,1));
				}
			}

			Sum.Apply(); // Применить
			Create(); // Создать объект
			WaterLine(); 
		}

		if (Input.GetKeyDown (KeyCode.Backspace)) { // Удаление объекта
			GameObject l = GameObject.FindGameObjectWithTag("Plane");
			if (l != null){
				Destroy(l);
			}

			Start();
		}
	}
}
