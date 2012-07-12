using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ServiceModel.Syndication;
using System.Windows.Threading;
using RssViewer.Utilities;
using System.Xml;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Threading;

namespace RssViewer.BackgroundWorkers
{
    public class Loader
    {
        private MWindow mWindow;
        private SyndicationFeed rssFeedChannel;

        public Loader(MWindow mainWindow)
        {
            this.mWindow = mainWindow;
        }

        public void InitializeChannels()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += this.worker_DoWork;
            worker.RunWorkerCompleted += this.worker_RunWorkerCompleted;
            worker.ProgressChanged += this.worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Action changeStatusState = () => this.mWindow.statusMessageBox.Text = "Loading channels...";
            this.mWindow.statusMessageBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, changeStatusState);
            BackgroundWorker worker = sender as BackgroundWorker;
            this.mWindow.feedList = FeedHelper.LoadFeeds(FeedHelper.LocateFeedData()).ToList();
            this.mWindow.channelList = new List<SyndicationFeed>();
            for (int i = 0; i < this.mWindow.feedList.Count; i++)
            {
                using (var xmlReader = XmlReader.Create(mWindow.feedList[i].Url))
                {
                    this.rssFeedChannel = SyndicationFeed.Load(xmlReader);
                }
                this.mWindow.feedList[i].RefreshNewItemsCount(this.rssFeedChannel);
                this.mWindow.channelList.Add(rssFeedChannel);
                worker.ReportProgress((int) (((double) (i + 1) / (double) this.mWindow.feedList.Count) * 100));
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) //Update UI when all feeds have been loaded.
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += new EventHandler(animation_Completed);
            this.mWindow.statusMessageBox.BeginAnimation(TextBox.OpacityProperty, animation);
            this.mWindow.progressBar.BeginAnimation(ProgressBar.OpacityProperty, animation);      
            for (int i = 0; i < this.mWindow.feedList.Count; i++)
            {
                this.mWindow.AddChannelPanel(this.mWindow.channelList[i].Title.Text + " (" + this.mWindow.feedList[i].NewItemsCount + ")", i);
            }   
        }

        private void animation_Completed(object sender, EventArgs e) //Hide progress bar when loading is finished
        {
            this.mWindow.progressBar.Visibility = Visibility.Hidden;
            this.mWindow.statusMessageBox.Text = "";
            this.mWindow.progressBar.Value = 0.0;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) //Update progress bar
        {
            this.mWindow.progressBar.Value = e.ProgressPercentage;
        }
    }
}
