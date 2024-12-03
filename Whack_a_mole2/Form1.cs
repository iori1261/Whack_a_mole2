using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Whack_a_mole2
{
    public partial class Form1 : Form
    {
        //フィールド変数
        List<PictureBox> pBoxList;
        int score;
        Random random;
        Image imgMoleEntry;
        Image imgMoleHit;
        Dictionary<PictureBox, Timer> hiddenTimers; // 各モグラの非表示タイマーを管理

        public Form1()
        {
            InitializeComponent();

            //PictureBoxの一覧を取得
            pBoxList = this.Controls.OfType<PictureBox>().ToList();

            //ランダム数の準備
            random = new Random();

            //画像ファイルの準備
            imgMoleEntry = Image.FromFile("C:\\Users\\236137\\source\\repos\\Whack_a_mole2\\Whack_a_mole2\\pic\\MoleEntry90.png");
            imgMoleHit = Image.FromFile("C:\\Users\\236137\\source\\repos\\Whack_a_mole2\\Whack_a_mole2\\pic\\MoleHit90.png");

            //タイマーの設定
            timGame.Interval = 20000;   // ゲーム全体の時間
            timDisp.Interval = 200;    // モグラが出現する間隔

            // 非表示タイマー用の辞書を初期化
            hiddenTimers = new Dictionary<PictureBox, Timer>();

            // PictureBoxクリックイベントを追加
            foreach (PictureBox pBox in pBoxList)
            {
                pBox.Click += new EventHandler(Mole_Click);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // ゲーム開始時の初期化
            foreach (PictureBox pBox in pBoxList)
            {
                pBox.Image = null;  // モグラを消去
            }
            score = 0;
            lblScore.Text = score.ToString();

            // タイマー開始
            timGame.Start();
            timDisp.Start();
        }

        private void timDisp_Tick(object sender, EventArgs e)
        {
            // モグラをランダムな位置に表示
            int index = random.Next(pBoxList.Count);
            PictureBox targetBox = pBoxList[index];

            if (targetBox.Image == null) // モグラが表示されていない場合
            {
                targetBox.Image = imgMoleEntry;

                // 非表示タイマーを設定
                if (!hiddenTimers.ContainsKey(targetBox))
                {
                    Timer hideTimer = new Timer();
                    hideTimer.Interval = 1000; // 1秒後にモグラを非表示
                    hideTimer.Tick += (s, ev) =>
                    {
                        targetBox.Image = null; // モグラを非表示
                        hideTimer.Stop();
                    };
                    hiddenTimers[targetBox] = hideTimer;
                }
                hiddenTimers[targetBox].Start();
            }
        }

        private void Mole_Click(object sender, EventArgs e)
        {
            // モグラをクリックしたときの処理
            PictureBox clickedMole = sender as PictureBox;
            if (clickedMole != null && clickedMole.Image == imgMoleEntry)
            {
                // モグラを叩いた場合の画像変更と得点追加
                clickedMole.Image = imgMoleHit;
                score += 10;
                lblScore.Text = score.ToString();

                // 非表示タイマーをリセット
                if (hiddenTimers.ContainsKey(clickedMole))
                {
                    hiddenTimers[clickedMole].Stop();
                    hiddenTimers[clickedMole].Start(); // 叩いた後も再び消えるまでの時間を設定
                }
            }
        }

        private void timGame_Tick(object sender, EventArgs e)
        {
            // ゲーム時間が終了した際の処理
            timGame.Stop();
            timDisp.Stop();

            foreach (var timer in hiddenTimers.Values)
            {
                timer.Stop();
            }

            MessageBox.Show("ゲーム終了！スコア: " + score.ToString(), "Whack-a-mole");
        }
    }
}
