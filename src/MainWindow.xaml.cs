﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using src.api;
using RiotSharp;

namespace src {

    public partial class MainWindow : Window {

        Client client;

        public MainWindow() {
            InitializeComponent();
            client = new Client();

			cbxRegion.ItemsSource = Enum.GetNames(typeof(Region)
			
			);
        }

        bool mouseDown = false;
        int lastX, lastY;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            mouseDown = true;
            lastX = (int) e.GetPosition(this).X;
            lastY = (int) e.GetPosition(this).Y;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            mouseDown = false;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDown) {
                int xDiff = (int)e.GetPosition(this).X - lastX;
                int yDiff = (int)e.GetPosition(this).Y - lastY;
                this.Left += xDiff;
                this.Top += yDiff;
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e) {
            mouseDown = false;
        }

		private void Menu_Click(object sender, EventArgs args)
		{
			if (true){
				var button = (sender as Button);
				switch(button.Content.ToString()){
					case "Summoner":
						
						break;
				}
			}
			else
			{
				content.Content = "Please insert a Summoner name and/or select a region.";
			}
		}
    }
}
