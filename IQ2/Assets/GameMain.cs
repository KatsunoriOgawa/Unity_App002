using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameMain : MonoBehaviour {

	private int lebel;

	// 画像配置数
	private static int[] imgMax = { 3, 5, 7 };

	private static Button btnPopup = null;
	private static Image imgPopup = null;

	// クリアフラグ
	private static bool isClear;

	private static float picSize = 188.0f;

	// 画像サイズ
	private Rect defaultPartsRect = new Rect(0.0f, 0.0f, picSize, picSize);

	// 
	private Image imgTemp;

	// 正解
	private struct ANSWERINFO
	{
		public Texture2D t2d;
		public int btnAnsFlag;
	};
	private static ANSWERINFO[] aInfos = null;

	private static Vector2[,,] postions = {
		{
			{
				new Vector2( 0, picSize ),
				new Vector2( -150, -picSize ),
				new Vector2( 150, -picSize ),
				new Vector2( 0, 0 ), // 配列数が多いのに合わせないといけいないので使わないけど定義・・・
				new Vector2( 0, 0 ), // 同上
				new Vector2( 0, 0 ), // 同上
				new Vector2( 0, 0 )  // 同上
			},{
				new Vector2( -150, picSize ),
				new Vector2( 150, 0 ),
				new Vector2( -150, -picSize ),
				new Vector2( 0, 0 ), // 配列数が多いのに合わせないといけいないので使わないけど定義・・・
				new Vector2( 0, 0 ), // 同上
				new Vector2( 0, 0 ), // 同上
				new Vector2( 0, 0 )  // 同上
			},{
				new Vector2( 150, picSize ),
				new Vector2( -150, 0 ),
				new Vector2( 150, -picSize ),
				new Vector2( 0, 0 ), // 配列数が多いのに合わせないといけいないので使わないけど定義・・・
				new Vector2( 0, 0 ), // 同上
				new Vector2( 0, 0 ), // 同上
				new Vector2( 0, 0 )  // 同上
			}
		},
		{
			{
				new Vector2( picSize, 250 ),
				new Vector2( -picSize, 250 ),
				new Vector2( 0, 0 ),
				new Vector2( picSize, -250 ),
				new Vector2( -picSize, -250 ),
				new Vector2( 0, 0 ), // 配列数が多いのに合わせないといけいないので使わないけど定義・・・
				new Vector2( 0, 0 )  // 同上
			},{
				new Vector2( 150, 300 ),
				new Vector2( 150, 0 ),
				new Vector2( -150, 150 ),
				new Vector2( -150, -150 ),
				new Vector2( 150, -300 ),
				new Vector2( 0, 0 ), // 配列数が多いのに合わせないといけいないので使わないけど定義・・・
				new Vector2( 0, 0 )  // 同上
			},{
				new Vector2( -150, 300 ),
				new Vector2( -150, 0 ),
				new Vector2( 150, 150 ),
				new Vector2( 150, -150 ),
				new Vector2( -150, -300 ),
				new Vector2( 0, 0 ), // 配列数が多いのに合わせないといけいないので使わないけど定義・・・
				new Vector2( 0, 0 )  // 同上
			}
		},
		{
			{
				new Vector2( picSize, 300 ),
				new Vector2( -picSize, 300 ),
				new Vector2( 0, 100 ),
				new Vector2( picSize, -100 ),
				new Vector2( -picSize, -100 ),
				new Vector2( picSize, -300 ),
				new Vector2( -picSize, -300 )
			},{
				new Vector2( 0, 300 ),
				new Vector2( -picSize, 150 ),
				new Vector2( picSize, 150 ),
				new Vector2( 0, 0 ),
				new Vector2( -picSize, -150 ),
				new Vector2( picSize, -150 ),
				new Vector2( 0, -300 )
			},{
				new Vector2( -picSize, 300 ),
				new Vector2( picSize, 300 ),
				new Vector2( 0, 150 ),
				new Vector2( -picSize, 0 ),
				new Vector2( picSize, 0 ),
				new Vector2( 0, -150 ),
				new Vector2( -picSize, -300 )
			}
		}
	};

	// 問題表示までのカウントダウン
	private int countDown;

	// 問題ID
	private int question_id;
	private string image_path;

	// 正解ボタン
	[SerializeField]
	private GameObject Popup;
	[SerializeField]
	private GameObject BtnPrefab;
	[SerializeField]
	private GameObject Canvas;
	[SerializeField]
	private GameObject Img;
	[SerializeField]
	private Text txtMessage;

	Timer timer = new Timer();

	private GameObject img1;
	private GameObject[] btn;

	// 開始
	void Start()
	{
		lebel = Common.getLevel ()-1;

		isClear = false;
		foreach (Transform child in GetComponent<Canvas>().transform){
			if(child.name == "btnPopup"){
				btnPopup = child.gameObject.GetComponent<Button>();
				btnPopup.gameObject.SetActive (false);
				foreach (Transform btnChild in btnPopup.transform){
					if(btnChild.name == "imgPopup"){
						imgPopup = btnChild.gameObject.GetComponent<Image> ();
					}
				}
			}
		}
		countDown = 3;
		timer.IsEnable = true;
		timer.LimitTime = 1;
		timer.FireDelegate = WaitQuestion;

		img1 = Instantiate (Img) as GameObject;
		img1.transform.SetParent (Canvas.transform,false);

		DataTable dt = Common.ExcuteDB("select * from ImgList ORDER BY RANDOM() LIMIT 1,1");
		image_path = null;

		foreach (DataRow dr in dt.Rows) {
			question_id = (int)dr["id"];
			image_path = (string)dr["image_path"];
			break;
		}

		imgTemp = img1.GetComponent<Image>();
		imgTemp.sprite = Sprite.Create(
			Resources.Load (countDown.ToString()) as Texture2D,
			defaultPartsRect,
			new Vector2(0.3f,0.3f),
			picSize);
		txtMessage.text = "「え」がひょうじされるまで！";
	}
	void Update() {
		if (timer.Update ()) {
			//FireDelegateを使わない場合は、この中でRecovery();を呼んでね

		}
	}

	// 問題を表示するまでのカウント
	void WaitQuestion()
	{
		countDown--;
		if (0 < countDown) {
			imgTemp = img1.GetComponent<Image>();
			imgTemp.sprite = Sprite.Create(
				Resources.Load (countDown.ToString()) as Texture2D,
				defaultPartsRect,
				new Vector2(0.3f,0.3f),
				picSize);
			timer.LimitTime = 1;
			timer.FireDelegate = WaitQuestion;
		} else {
			txtMessage.text = "この「え」をおぼえよう！";
			imgTemp = img1.GetComponent<Image>();
			imgTemp.sprite = Sprite.Create(
				Resources.Load (image_path) as Texture2D,
				defaultPartsRect,
				new Vector2(1.0f,1.0f),
				picSize);
			Debug.Log ( image_path );
			timer.LimitTime = 1;
			timer.FireDelegate = Question;
		}
	}

	// 問題を表示
	void Question()
	{
		DataTable dt = Common.ExcuteDB( "SELECT * FROM " +
			"(SELECT im1.id, im1.image_path, 1 AS ansflag " +
			"FROM ImgList AS im1 " +
			"WHERE im1.id = '"+question_id+"' " +
			"UNION ALL " +
			"SELECT * FROM " +
			"(SELECT im2.id, im2.image_path, 0 AS ansflag " +
			"FROM ImgList AS im2 " +
			"WHERE im2.id != '"+question_id+"' " +
			"ORDER BY RANDOM() LIMIT "+(imgMax[lebel]-1)+" )) " +
			"ORDER BY RANDOM()"
		);

		int ansCnt = 0;
		aInfos = new ANSWERINFO[imgMax[lebel]];
		// 正解データ取得のSQL
		foreach (DataRow dr in dt.Rows) {
			Debug.Log ( (int)dr["ansflag"] );
			aInfos[ansCnt].btnAnsFlag = (int)dr["ansflag"];
			image_path = (string)dr["image_path"];
			aInfos[ansCnt].t2d = Resources.Load (image_path) as Texture2D;
			ansCnt++;
		}
		timer.LimitTime = 5-lebel*2;
		timer.FireDelegate = Anser;
	}

	// 答えを選択
	void Anser()
	{
		txtMessage.text = "さっきみた「え」はどれかな？";
		Destroy (img1);
		timer.IsEnable = false;
		int idx = Random.Range (0, 3);
		btn = new GameObject[imgMax[lebel]];  // GameObjectの配列を作成.

		for( int i = 0;i < imgMax[lebel];i++ ){
			btn[i] = Instantiate (BtnPrefab) as GameObject;
			btn[i].transform.SetParent (Canvas.transform,false);

			//ボタンの位置を修正するためにRectTransformコンポーネントを取得
			RectTransform btnRectTrans = btn[i].GetComponent<RectTransform>();
			//ボタンの位置はlocalPositionで指定できる
			Debug.Log("lebel"+lebel+" idx:"+idx+" i:"+i);
			//Debug.Log(postions[lebel,idx,i]);
			Debug.Log(postions[lebel,idx,i]);
			btnRectTrans.localPosition = postions[lebel,idx,i];

			ButtonScript b = btn[i].GetComponent<ButtonScript>();
			b.id = i;

			Image btnImg = btn[i].GetComponent<Image>();
			btnImg.sprite = Sprite.Create(
				aInfos [i].t2d,
				defaultPartsRect,
				new Vector2(1.0f,1.0f),
				picSize);
		}
	}

	public static void CheckAnswer (int idx){
		int level = Common.getLevel()-1;
		if (level < 0) level = 0;

		// 押されたボタンが配列外なら終了
		if ( imgMax [level] <= idx ) {
			return;
		}
		string pngName;
		Rect rect;
		if (aInfos [idx].btnAnsFlag != 0) {
			Common.addGoodCnt ();
			if (5 <= Common.getGoodCnt ()) {
				pngName = "0052";
				isClear = true;
				SoundManager.Instance.PlaySE ( 3 );
				rect = new Rect (0.0f, 0.0f, 300.0f, 110.0f);
			} else {
				pngName = "0051";
				SoundManager.Instance.PlaySE ( 1 );
				rect = new Rect (0.0f, 0.0f, 300.0f, 99.0f);
			}
		} else {
			pngName = "0050";
			Common.addBadCnt ();
			SoundManager.Instance.PlaySE ( 2 );
			rect = new Rect (0.0f, 0.0f, 300.0f, 99.0f);
		}
		btnPopup.gameObject.SetActive (true);
		imgPopup.sprite = Sprite.Create(
			Resources.Load( pngName ) as Texture2D,
			rect,
			new Vector2(0.3f,0.3f),
			0.0f);
	}

	// ポップアップをクリック
	public static void PopupClick (){
		if (!isClear) {
			Application.LoadLevel ("GameMain");
		} else {
			Application.LoadLevel ("TopMenu");
		}
	}
}
