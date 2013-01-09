﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.Model.AirportModel;
using TheAirline.Model.GeneralModel;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirlinerModel;
using TheAirline.GraphicsModel.Converters;
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.GraphicsModel.UserControlModel.MessageBoxModel;
using TheAirline.GraphicsModel.UserControlModel.PopUpWindowsModel;
using TheAirline.Model.PassengerModel;
using TheAirline.Model.GeneralModel.Helpers;

namespace TheAirline.GraphicsModel.PageModel.PageRouteModel.PanelRoutesModel
{
    public class PanelNewRoute : StackPanel
    {
        private TextBlock txtDistance,  txtFlightCode, txtInvalidRoute,txtFlightRestrictions, txtDestination1Gates, txtDestination2Gates;
        private ComboBox cbDestination1, cbDestination2;
        private Button btnSave, btnLoad;
        private PageRoutes ParentPage;
        private ucStopover ucStopover1, ucStopover2;
        private double MaxDistance;
        private Dictionary<AirlinerClass.ClassType, RouteAirlinerClass> Classes;
        public PanelNewRoute(PageRoutes parent)
        {
            this.Classes = new Dictionary<AirlinerClass.ClassType, RouteAirlinerClass>();

            var query = from a in AirlinerTypes.GetTypes(delegate(AirlinerType t) { return t.Produced.From < GameObject.GetInstance().GameTime; })
                        select a.Range;

            this.MaxDistance = query.Max();

            this.ParentPage = parent;

            this.Margin = new Thickness(0, 0, 50, 0);
         
            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = Translator.GetInstance().GetString("PanelNewRoute", "201");
            this.Children.Add(txtHeader);

            ListBox lbRouteInfo = new ListBox();
            lbRouteInfo.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbRouteInfo.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");

            this.Children.Add(lbRouteInfo);

            WrapPanel panelDestination1 = new WrapPanel();

            cbDestination1 = createDestinationComboBox(); 
            panelDestination1.Children.Add(cbDestination1);

            txtDestination1Gates = new TextBlock();
            txtDestination1Gates.Margin = new Thickness(5, 0, 0, 0);
            txtDestination1Gates.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            panelDestination1.Children.Add(txtDestination1Gates);

            lbRouteInfo.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PanelNewRoute", "202"), panelDestination1));

            WrapPanel panelDestination2 = new WrapPanel();
                        
            cbDestination2 = createDestinationComboBox();
            panelDestination2.Children.Add(cbDestination2);

            txtDestination2Gates = new TextBlock();
            txtDestination2Gates.Margin = new Thickness(5, 0, 0, 0);
            txtDestination2Gates.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            panelDestination2.Children.Add(txtDestination2Gates);

            lbRouteInfo.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PanelNewRoute", "203"), panelDestination2));

            ucStopover1 = new ucStopover();
            ucStopover1.ValueChanged += ucStopover_OnValueChanged;
            lbRouteInfo.Items.Add(new QuickInfoValue("Stopover", ucStopover1));

            ucStopover2 = new ucStopover();
            ucStopover2.ValueChanged += ucStopover_OnValueChanged;
            lbRouteInfo.Items.Add(new QuickInfoValue("Stopover", ucStopover2));
         
            txtDistance = UICreator.CreateTextBlock("-");
            lbRouteInfo.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PanelNewRoute", "204"), txtDistance));
            lbRouteInfo.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PanelNewRoute", "205"), UICreator.CreateTextBlock(string.Format("{0:0} {1}", new NumberToUnitConverter().Convert(this.MaxDistance), new StringToLanguageConverter().Convert("km.")))));

      
            foreach (AirlinerClass.ClassType type in Enum.GetValues(typeof(AirlinerClass.ClassType)))
            {
                
                RouteAirlinerClass rClass = new RouteAirlinerClass(type,RouteAirlinerClass.SeatingType.Reserved_Seating, 1);

                foreach (RouteFacility.FacilityType ftype in Enum.GetValues(typeof(RouteFacility.FacilityType)))
                {
                    if (GameObject.GetInstance().GameTime.Year >= (int)ftype)
                        rClass.addFacility(RouteFacilities.GetBasicFacility(ftype));
                }
             
                this.Classes.Add(type, rClass);

                WrapPanel panelClassButtons = new WrapPanel();

                Button btnEdit = new Button();
                btnEdit.Background = Brushes.Transparent;
                btnEdit.Tag = type;
                btnEdit.Click += new RoutedEventHandler(btnEdit_Click);
      
                Image imgEdit = new Image();
                imgEdit.Width = 16;
                imgEdit.Source = new BitmapImage(new Uri(@"/Data/images/edit.png", UriKind.RelativeOrAbsolute));
                RenderOptions.SetBitmapScalingMode(imgEdit, BitmapScalingMode.HighQuality);

                btnEdit.Content = imgEdit;

                panelClassButtons.Children.Add(btnEdit);

                Image imgInfo = new Image();
                imgInfo.Width = 16;
                imgInfo.Source = new BitmapImage(new Uri(@"/Data/images/info.png", UriKind.RelativeOrAbsolute));
                imgInfo.Margin = new Thickness(5, 0, 0, 0);
                imgInfo.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                RenderOptions.SetBitmapScalingMode(imgInfo, BitmapScalingMode.HighQuality);

                Border brdToolTip = new Border();
                brdToolTip.Margin = new Thickness(-4, 0, -4, -3);
                brdToolTip.Padding = new Thickness(5);
                brdToolTip.SetResourceReference(Border.BackgroundProperty, "HeaderBackgroundBrush2");


                ContentControl lblClass= new ContentControl();
                lblClass.SetResourceReference(ContentControl.ContentTemplateProperty, "RouteAirlinerClassItem");
                lblClass.Content = rClass;

                brdToolTip.Child = lblClass;


                imgInfo.ToolTip = brdToolTip;

                panelClassButtons.Children.Add(imgInfo);

       

                lbRouteInfo.Items.Add(new QuickInfoValue(new TextUnderscoreConverter().Convert(type, null, null, null).ToString(), panelClassButtons));
            }

            txtFlightCode = new TextBlock();

            WrapPanel panelButtons = new WrapPanel();
            panelButtons.Margin = new Thickness(0, 5, 0, 0);
            this.Children.Add(panelButtons);

            btnSave = new Button();
            btnSave.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnSave.Height = Double.NaN;
            btnSave.Width = Double.NaN;
            btnSave.Content = Translator.GetInstance().GetString("General","113");
            btnSave.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnSave.Click += new RoutedEventHandler(btnSave_Click);
            btnSave.IsEnabled = false;
            panelButtons.Children.Add(btnSave);

            btnLoad = new Button();
            btnLoad.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnLoad.Height = Double.NaN;
            btnLoad.Width = Double.NaN;
            btnLoad.Content = Translator.GetInstance().GetString("General", "115");
            btnLoad.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnLoad.IsEnabled = false;
            btnLoad.Click += new RoutedEventHandler(btnLoad_Click);
            btnLoad.Margin = new Thickness(5, 0, 0, 0);
            panelButtons.Children.Add(btnLoad);

            txtInvalidRoute = UICreator.CreateTextBlock(Translator.GetInstance().GetString("PanelNewRoute","1001"));
            txtInvalidRoute.Foreground = Brushes.DarkRed;
            txtInvalidRoute.Visibility = System.Windows.Visibility.Collapsed;
            this.Children.Add(txtInvalidRoute);

            txtFlightRestrictions = UICreator.CreateTextBlock("");
            txtFlightRestrictions.Foreground = Brushes.DarkRed;
            txtFlightRestrictions.Visibility = System.Windows.Visibility.Collapsed;
            this.Children.Add(txtFlightRestrictions);

        }

        private void ucStopover_OnValueChanged(Airport airport)
        {
            cbDestination_SelectionChanged(airport,null);
        }

       
      
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            AirlinerClass.ClassType type = (AirlinerClass.ClassType)((Button)sender).Tag;
            RouteAirlinerClass aClass = (RouteAirlinerClass)PopUpRouteFacilities.ShowPopUp(this.Classes[type]);

            if (aClass != null)
            {
                this.Classes[type].FarePrice = aClass.FarePrice;
                this.Classes[type].Seating = aClass.Seating;

                foreach (RouteFacility facility in aClass.getFacilities())
                    this.Classes[type].addFacility(facility);
            }
        }
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ComboBox cbConfigurations = new ComboBox();
            cbConfigurations.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbConfigurations.SelectedValuePath = "Name";
            cbConfigurations.DisplayMemberPath = "Name";
            cbConfigurations.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbConfigurations.Width = 200;

            foreach (RouteClassesConfiguration confItem in Configurations.GetConfigurations(Configuration.ConfigurationType.Routeclasses))
                cbConfigurations.Items.Add(confItem);

            cbConfigurations.SelectedIndex = 0;

            if (PopUpSingleElement.ShowPopUp("Select configuration", cbConfigurations) == PopUpSingleElement.ButtonSelected.OK && cbConfigurations.SelectedItem != null)
            {
                RouteClassesConfiguration configuration = (RouteClassesConfiguration)cbConfigurations.SelectedItem;

                foreach (RouteClassConfiguration classConfiguration in configuration.getClasses())
                {
                    foreach (RouteFacility facility in classConfiguration.getFacilities())
                        this.Classes[classConfiguration.Type].addFacility(facility);
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            Airline airline = GameObject.GetInstance().HumanAirline;
            Airport dest1 = (Airport)cbDestination1.SelectedItem;
            Airport dest2 = (Airport)cbDestination2.SelectedItem;
            Airport stopover1 = ucStopover1.Value;
            Airport stopover2 = ucStopover2.Value;

            Boolean stopoverOk = (stopover1 == null ? true : stopover1.Terminals.getFreeGates(airline) > 0) && (stopover2 == null ? true : stopover2.Terminals.getFreeGates(airline) > 0);

            if (dest1.Terminals.getFreeGates(airline) > 0 && dest2.Terminals.getFreeGates(airline) > 0 && stopoverOk)
            {

                Guid id = Guid.NewGuid();
                Route route = new Route(id.ToString(),dest1, dest2, 0);

                foreach (RouteAirlinerClass aClass in this.Classes.Values)
                {
                    route.getRouteAirlinerClass(aClass.Type).FarePrice = aClass.FarePrice;

                    foreach (RouteFacility facility in aClass.getFacilities())
                        route.getRouteAirlinerClass(aClass.Type).addFacility(facility);
                   
                    route.getRouteAirlinerClass(aClass.Type).Seating = aClass.Seating;
        
                }

                if (stopover1 != null)
                    route.addStopover(stopover1);

                if (stopover2 != null)
                    route.addStopover(stopover2);

                airline.addRoute(route);

                dest1.Terminals.getEmptyGate(airline).HasRoute = true;
                dest2.Terminals.getEmptyGate(airline).HasRoute = true;

                this.ParentPage.showRoutes();

                this.Visibility = System.Windows.Visibility.Collapsed;

                route.LastUpdated = GameObject.GetInstance().GameTime; 
            }
            else
                WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2501"), Translator.GetInstance().GetString("MessageBox", "2501", "message"), WPFMessageBoxButtons.Ok);

        }

        //creates the combo box for selecting a primary route aircraft
        private ComboBox createAircraftComboBox()
        {
            ComboBox cbAircraft = new ComboBox();

            cbAircraft.SetResourceReference(ComboBox.ItemTemplateProperty, "AirportCountryItem");
            cbAircraft.Background = Brushes.Transparent;
            cbAircraft.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbAircraft.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbAircraft.SelectionChanged += new SelectionChangedEventHandler(cbAircraft_SelectionChanged);
            List<FleetAirliner> airliners = GameObject.GetInstance().HumanAirline.Fleet.FindAll((delegate(FleetAirliner a) { return a.Airliner.BuiltDate <= GameObject.GetInstance().GameTime; }));
       
            foreach (FleetAirliner airliner in airliners)
                cbAircraft.Items.Add(airliner);
        
            return cbAircraft;
        }

        private void cbAircraft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        //creates the combo box for a destination
        private ComboBox createDestinationComboBox()
        {
            ComboBox cbDestination = new ComboBox();

            cbDestination.SetResourceReference(ComboBox.ItemTemplateProperty, "AirportCountryItem");
            cbDestination.Background = Brushes.Transparent;
            cbDestination.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbDestination.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbDestination.SelectionChanged += new SelectionChangedEventHandler(cbDestination_SelectionChanged);
            List<Airport> airports = GameObject.GetInstance().HumanAirline.Airports.FindAll(a=>a.Terminals.getFreeGates(GameObject.GetInstance().HumanAirline)>0);
            airports.Sort(delegate(Airport a1, Airport a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); });

            foreach (Airport airport in airports)
                cbDestination.Items.Add(airport);

            return cbDestination;


        }

        private void cbDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDestination2.SelectedItem != null && cbDestination1.SelectedItem != null)
            {
                Airport airport1 = (Airport)cbDestination1.SelectedItem;
                Airport airport2 = (Airport)cbDestination2.SelectedItem;
                Airport stopover1 = ucStopover1.Value;
                Airport stopover2 = ucStopover2.Value;

                List<double> distances = new List<double>();
                Boolean isRouteOk = false;

                if (stopover1 == null && stopover2 == null)
                {
                    distances.Add(MathHelpers.GetDistance(airport1, airport2));
                    isRouteOk = checkRouteOk(airport1, airport2);
                }

                if (stopover1 == null && stopover2 != null)
                {
                    distances.Add(MathHelpers.GetDistance(airport1, stopover2));
                    distances.Add(MathHelpers.GetDistance(stopover2, airport2));
                    isRouteOk = checkRouteOk(airport1, stopover2) && checkRouteOk(stopover2, airport2);
                }

                if (stopover1 != null && stopover2 == null)
                {
                    distances.Add(MathHelpers.GetDistance(airport1, stopover1));
                    distances.Add(MathHelpers.GetDistance(stopover1, airport2));
                    isRouteOk = checkRouteOk(airport1, stopover1) && checkRouteOk(stopover1, airport2);
                }

                if (stopover1 != null && stopover2 != null)
                {
                    distances.Add(MathHelpers.GetDistance(airport1, stopover1));
                    distances.Add(MathHelpers.GetDistance(stopover1, stopover2));
                    distances.Add(MathHelpers.GetDistance(stopover2, airport2));
                    isRouteOk = checkRouteOk(airport1, stopover1) && checkRouteOk(stopover1,stopover2)  && checkRouteOk(stopover2, airport2);
                }


                foreach (RouteAirlinerClass aClass in this.Classes.Values)
                {
                    
                    aClass.FarePrice = PassengerHelpers.GetPassengerPrice(airport1, airport2) * GeneralHelpers.ClassToPriceFactor(aClass.Type);
                }

                double maxDistance = distances.Max();
                double minDistance = distances.Min();

                txtDistance.Text = string.Format("{0:0} {1}", new NumberToUnitConverter().Convert(maxDistance), new StringToLanguageConverter().Convert("km."));

                btnSave.IsEnabled = minDistance > 50 && maxDistance < this.MaxDistance && isRouteOk;
                btnLoad.IsEnabled = btnSave.IsEnabled;
    
                txtInvalidRoute.Visibility = AIHelpers.IsRouteInCorrectArea(airport1,airport2) ? Visibility.Collapsed : Visibility.Visible;
                
                txtFlightRestrictions.Visibility =FlightRestrictions.HasRestriction(airport2.Profile.Country,airport1.Profile.Country,GameObject.GetInstance().GameTime,FlightRestriction.RestrictionType.Flights) ||  FlightRestrictions.HasRestriction(GameObject.GetInstance().HumanAirline,airport1.Profile.Country,airport2.Profile.Country,GameObject.GetInstance().GameTime) || FlightRestrictions.HasRestriction(airport1.Profile.Country,airport2.Profile.Country,GameObject.GetInstance().GameTime,FlightRestriction.RestrictionType.Flights) ? Visibility.Visible : System.Windows.Visibility.Collapsed;

                txtFlightRestrictions.Text= string.Format(Translator.GetInstance().GetString("PanelNewRoute","1002"),airport1.Profile.Country.Name,airport2.Profile.Country.Name);

            }

            if (sender is ComboBox)
            {
                Airport airport = (Airport)((ComboBox)sender).SelectedItem;

                TextBlock txtDestinationGates = cbDestination2 == ((ComboBox)sender) ? txtDestination2Gates : txtDestination1Gates;

                txtDestinationGates.Text = string.Format(Translator.GetInstance().GetString("PanelNewRoute", "206"), airport.Terminals.getFreeGates(GameObject.GetInstance().HumanAirline));
            }
       
        }
        //returns if two airports can have route between them
        private Boolean checkRouteOk(Airport airport1, Airport airport2)
        {
           return AIHelpers.IsRouteInCorrectArea(airport1, airport2) && !FlightRestrictions.HasRestriction(airport1.Profile.Country, airport2.Profile.Country, GameObject.GetInstance().GameTime, FlightRestriction.RestrictionType.Flights) && !FlightRestrictions.HasRestriction(airport2.Profile.Country, airport1.Profile.Country, GameObject.GetInstance().GameTime, FlightRestriction.RestrictionType.Flights) && !FlightRestrictions.HasRestriction(GameObject.GetInstance().HumanAirline, airport1.Profile.Country, airport2.Profile.Country, GameObject.GetInstance().GameTime);
        }
       //class for a stop over item
        private class ucStopover : UserControl
        {
            public Airport Value { get; set; }
            private ComboBox cbDestination;
            public delegate void OnValueChanged(Airport airport);
            public event OnValueChanged ValueChanged;
      
            public ucStopover()
            {
                WrapPanel panelStopover = new WrapPanel();

                cbDestination = new ComboBox();
                cbDestination.SetResourceReference(ComboBox.ItemTemplateProperty, "AirportCountryItem");
                cbDestination.Background = Brushes.Transparent;
                cbDestination.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
                cbDestination.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                cbDestination.SelectionChanged += cbDestination_SelectionChanged;

                List<Airport> airports = GameObject.GetInstance().HumanAirline.Airports.FindAll(a => a.Terminals.getFreeGates(GameObject.GetInstance().HumanAirline) > 0);
                airports.Sort(delegate(Airport a1, Airport a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); });

                foreach (Airport airport in airports)
                    cbDestination.Items.Add(airport);

                panelStopover.Children.Add(cbDestination);


                Button btnDelete = new Button();
                btnDelete.Click += btnDelete_Click;
                btnDelete.Margin = new Thickness(5, 0, 0, 0);
                btnDelete.Background = Brushes.Transparent;
             
                Image imgEdit = new Image();
                imgEdit.Width = 16;
                imgEdit.Source = new BitmapImage(new Uri(@"/Data/images/delete.png", UriKind.RelativeOrAbsolute));
                RenderOptions.SetBitmapScalingMode(imgEdit, BitmapScalingMode.HighQuality);

                btnDelete.Content = imgEdit;

                panelStopover.Children.Add(btnDelete);            

                this.Content = panelStopover;

            }

            private void btnDelete_Click(object sender, RoutedEventArgs e)
            {
                this.Value = null;
                cbDestination.SelectedItem = null;
            }

            private void cbDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                this.Value = (Airport)cbDestination.SelectedItem;

                if (this.ValueChanged != null)
                    this.ValueChanged(this.Value); 
   
            }
        }
    }
}
