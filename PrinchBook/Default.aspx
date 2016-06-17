<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="Default.aspx.cs" Inherits="PrinchBook.Default" ClientIDMode="Static" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="http://code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
    <asp:ScriptManager runat="server" ID="ScriptManager" EnableCdn="true" EnablePageMethods="true" />
    <script type="text/javascript" language="javascript">
        function toggleSelection(source) {

            var isChecked = source.checked;
            var container = document.getElementById("content");
            var inputs = container.getElementsByTagName("input");

            for (var i = 0; i < inputs.length; i++) {

                if (inputs[i].type = "checkbox") {

                    inputs[i].checked = false;

                }
            }

            source.checked = true;
        }

        function mouseOver(object) {

            var available = true;
            var pass = "<%=MyValue%>"

            var title2;

            var elements = document.getElementById('<%=clock.ClientID%>').children;

            for (var i = 0; i < elements.length; i++) {

                var thisButton = elements[i];

                if (thisButton == object) {
                    var firstButton = thisButton;
                    var nextButtons = parseInt(i) + parseInt(pass);

                    for (var j = i ; j <= nextButtons; j++) {

                        var colloredButtons = elements[j];

                        if (colloredButtons != null) {


                            var col = elements[j].style.backgroundColor;

                            if (colloredButtons != null && col != "red") {
                                if (col != "blue") {
                                    elements[j].style.backgroundColor = "darkgreen";
                                }

                                var lastButton = elements[j];
                                title2 = $(lastButton).attr("title");

                            }

                            else {

                                available = false;
                            }

                        } else if (colloredButtons == null) {
                            title2 = "22:15:00";
                        }
                    }
                }

            }



            var title1 = $(firstButton).attr("title");


            if (available) {
                console.log("available");
                var label = title1 + "-" + title2;
            } else {
                console.log("available not");
                var label = "Unavailable";
            }

            changeLable(label);

        }
        function mouseOut(object) {

            var pass = "<%=MyValue%>"
            var button = object;


            var res = button.id;
            var res2 = parseInt(pass);
            var elements = document.getElementById('<%=clock.ClientID%>').children;


            for (var i = 0; i < elements.length; i++) {

                var thisButton = elements[i];

                if (thisButton == object) {
                    var firstButton = thisButton;
                    var nextButtons = parseInt(i) + parseInt(pass);

                    for (var j = i ; j <= nextButtons; j++) {

                        var colloredButtons = elements[j];
                        if (colloredButtons != null) {
                            var col = colloredButtons.style.backgroundColor;

                            if (colloredButtons != null && col != "red" && col != "blue") {

                                colloredButtons.style.backgroundColor = "lightgreen";

                            }
                        }
                    }
                }

            }

        }

        function changeLable(time) {

            var label = document.getElementById('<%=time.ClientID%>');
            label.style.visibility = "visible";
            label.innerHTML = time;
        }

        function onmouseOverButton(object) {
            var elements = document.getElementById('<%=duration.ClientID%>').children;

            var button = document.getElementById(object.id);
            for (var i = 0; i < elements.length; i++) {
                if (object == elements[i]) {

                    elements[i].style.backgroundColor = "lightgreen";
                }

            }

        }
        function onMouseOutButton(object) {
            var elements = document.getElementById('<%=duration.ClientID%>').children;
            var pass = parseInt("<%=MyValue%>");

            var button = document.getElementById(object.id);
            if (button != elements[0] && pass == 2) {
                button.style.backgroundColor = "";
            }
            if (pass == 4 && object == elements[elements.length - 1]) {

                object.style.backgroundColor = "";
            }

        }
        function timePick(object) {

            var hidden = document.getElementById("hidden");
            var time = document.getElementById('<%=time.ClientID%>');
            var tooltip = object.title;
            hidden.value = time.innerHTML;




        }
        function rowMouseOver(object) {
            object.style.backgroundColor = "darkgreen";

            var i = object.children;
            var img = document.getElementsByClassName("image");
            var im = i[0].children;

            for (var j = 0; j < img.length; j++) {

                if (im[0] == img[j]) {

                    img[j].src = "Images/checked.png";
                    img[j].height = 25;
                    img[j].width = 25;
                }
            }





        }
        function rowMouseOut(object) {
            object.style.backgroundColor = "";

            var i = object.children;
            var img = document.getElementsByClassName("image");
            var im = i[0].children;

            for (var j = 0; j < img.length; j++) {

                if (im[0] == img[j]) {

                    img[j].src = "Images/unchecked.png";
                    img[j].height = 25;
                    img[j].width = 25;
                }
            }
        }
        function rowFunctionSelected(object) {

            var row_elements = object.children;
            var label = row_elements[1];
            var button = document.getElementById("PcType");
            button.value = label.innerText;
            var functionAtt = document.getElementById("hidden_functions");
            functionAtt.value = label.innerText;


            $find("ModalBehaviour").hide();
            theForm.submit();
        }



    </script>

    <link href="Css/StyleSheet.css" rel="stylesheet" type="text/css" />


    <asp:HiddenField ID="hidden" runat="server" />
    <asp:HiddenField ID="hidden_functions" runat="server" />

    <div class="content">

        <asp:Table runat="server" HorizontalAlign="Center" CellPadding="10">
            <asp:TableRow ID="TableRow3step3" HorizontalAlign="Center">
                <asp:TableCell>
                     <asp:Label runat="server" Text="Please choose date and time"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Calendar runat="server" ID="calendar"
                        OnDayRender="calendar_DayRender"
                        PrevMonthText="<img src='Images/previous-button.png' height=25>"
                        OnSelectionChanged="calendar_SelectionChanged" NextMonthText="<img src='Images/next-button.png' height=25"
                        FirstDayOfWeek="Monday" TodayDayStyle-ForeColor="Green"
                        TodayDayStyle-BackColor="Tan"
                        SelectedDayStyle-BorderWidth="1px"
                        SelectedDayStyle-ForeColor="Green"
                        CssClass="calendar" BorderStyle="Groove" OnVisibleMonthChanged="calendar_VisibleMonthChanged" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="hiddenLabel" runat="server" />

                </asp:TableCell>
                <asp:TableCell Style="text-align: center">

                    <div id="divPopUp" style="border-radius: 5px; background-color: aqua; display: none">
                        <div>
                            <asp:Repeater runat="server" ID="Functions">
                                <ItemTemplate>
                                    <asp:Table runat="server" ID="TbFunctions" GridLines="Horizontal" BorderStyle="Solid" BorderColor="Gray" HorizontalAlign="Center">
                                        <asp:TableRow onmouseover="rowMouseOver(this)" onmouseout="rowMouseOut(this)" Height="25" Width="25" ID="tbRow" onclick="rowFunctionSelected(this)">
                                            <asp:TableCell>
                                                <asp:Image runat="server" ID="Image" class="image" src="Images/unchecked.png" Style="height: 25px; width: 25px;" />
                                            </asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="LabelFunction" class="label" Text="<%#Container.DataItem.ToString() %>" Width="120" Style="align-content: center" />
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                    <ajaxToolkit:ModalPopupExtender ID="PCTypeTable" runat="server" TargetControlID="hiddenLabel" DropShadow="true" PopupControlID="divPopUp" BackgroundCssClass="modalBackground" Enabled="true" BehaviorID="ModalBehaviour" />
                </asp:TableCell>
            </asp:TableRow>


        </asp:Table>
        <asp:Table runat="server" HorizontalAlign="Center" Style="table-layout: fixed">
            <asp:TableRow ID="timepicker" Style="visibility: hidden">
                <asp:TableCell Width="20%">

                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Center" VerticalAlign="Middle" Width="25%">
                    <asp:Label runat="server" Text="Please choose the duration and starting time" />
                    <br />
                    <br />
                    <div runat="server" id="duration">
                        <asp:Button runat="server" ID="duration1" Text="30 min" BackColor="LightGreen" onmouseover="onmouseOverButton(this)" onmouseout="onMouseOutButton(this)" OnClick="duration_Click" />
                        <asp:Button runat="server" ID="duration2" Text="1 h" onmouseover="onmouseOverButton(this)" onmouseout="onMouseOutButton(this)" OnClick="duration_Click" />
                        <asp:Button runat="server" ID="duration3" Text="2 h" onmouseover="onmouseOverButton(this)" onmouseout="onMouseOutButton(this)" OnClick="duration_Click" />
                    </div>
                    <br />

                    <asp:Label runat="server" Text="8:00" Style="font-size: 15px; display: inline-block;" />
                    <div runat="server" id="clock" style="display: inline-block;" />
                    <asp:Label runat="server" Text="22:00" Style="font-size: 15px; display: inline-block;" />
                    <br />

                    <asp:Label runat="server" ID="time" Style="font-size: 15px; font: bold; visibility: hidden" Text="-" />
                </asp:TableCell>
                <asp:TableCell Width="1%" HorizontalAlign="Center">

                </asp:TableCell>
                <asp:TableCell Width="20%" HorizontalAlign="Center" VerticalAlign="Top">
                    <asp:Label runat="server" Text="Please choose functionality of computer if you want specific computer" />
                    <br />
                    <br />
                    <asp:Button runat="server" ID="PcType" Text="Choose" OnClick="PcType_Click" Width="120" Style="align-content: center" />
                </asp:TableCell>
                <asp:TableCell Width="20%"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <asp:Table runat="server" HorizontalAlign="Center" Style="table-layout: fixed">
            <asp:TableRow>
                <asp:TableCell Visible="false" ID="InfoCell" HorizontalAlign="Center">
                    <asp:Label runat="server" ID="FinishLabel" />
                    <br />
                    <br />
                    <asp:Button runat="server" Text="Finish" Style="border-radius: 15px" ID="FinishButton" OnClick="FinishButton_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="hiddenLabelfinish" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>

                <asp:TableCell Style="text-align: center">

                    <div id="divPopUpFinish" style="border-radius: 5px; background-color: aqua;display:none;padding:5px;">
                        <div id="main" style="padding:5px;">
                            <asp:Label ID="FinishInfo" runat="server" />
                        </div>
                        <div id="buttons">
                            <div id="DivbtnOK" style="width: 90%; margin: 0 auto">
                                <asp:Button ID="BooKPC" runat="server" Text="Book" OnClick="BooKPC_Click" Style="margin-bottom: 10px; border-radius: 5px" />
                                <asp:Button ID="Cancel" runat="server" Text="Cancel" Style="margin-bottom: 10px; border-radius: 5px" OnClick="Cancel_Click" />
                            </div>
                        </div>
                    </div>
                    <ajaxToolkit:ModalPopupExtender ID="FinishPopupExtender" runat="server" TargetControlID="hiddenLabelfinish" DropShadow="true" PopupControlID="divPopUpFinish" BackgroundCssClass="modalBackground" Enabled="true" BehaviorID="ModalBehaviour2" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>







</asp:Content>

