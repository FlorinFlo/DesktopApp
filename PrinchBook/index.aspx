<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" MasterPageFile="~/Site1.Master" Inherits="PrinchBook.index" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="Css/StyleSheet.css" rel="stylesheet" type="text/css" />
    
    <asp:ScriptManager runat="server" ID="ScriptManager" EnableCdn="true" />

    <div class="content">
        <asp:UpdatePanel ID="area_selection" runat="server">
            <ContentTemplate>
                <asp:Table runat="server" HorizontalAlign="Center" CellPadding="10">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Label runat="server" Text=" Please choose the area in which you would like to book a pc" ID="step1Lbl" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="Center">
                            <asp:DropDownList runat="server" ID="listArea" OnSelectedIndexChanged="listArea_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="-Select-" Value="" />
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow HorizontalAlign="Center">
                        <asp:TableCell>
                            <asp:Label runat="server" Text="Please choose desired library" ID="lblLibrary" Visible="false" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell HorizontalAlign="Center" >
                            <asp:DropDownList runat="server" ID="libraryList" OnSelectedIndexChanged="libraryList_SelectedIndexChanged" AutoPostBack="true" Visible="false" EnableViewState="true">
                                <asp:ListItem Text="-Select-" Value="" />
                            </asp:DropDownList>

                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Label ID="lblHidden" runat="server" Text="" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow >
                        <asp:TableCell style="text-align:center" >
                           
                            <ajaxToolkit:ModalPopupExtender ID="signinPopUp" runat="server" TargetControlID="lblHidden" DropShadow="true" PopupControlID="divPopUp" />

                            <div id="divPopUp" style="background-color: #169ED9; display: none; border-radius: 5px; position:absolute;top:150px">
                                <div id="Header" style="width: 60%; margin: 0 auto">Sign in form</div>
                                <div id="main" style="margin-left: auto; margin-right: auto; text-align: center;">
                                    <asp:Label runat="server" Text="User" Style="text-align: center" />
                                    <br />
                                    <asp:TextBox runat="server" ID="userNameText" Width="120" Style="text-align: center; margin: 10px" />
                                    <br />
                                    <asp:Label runat="server" Text="Password" Style="width: 50%; margin: 0 auto" />
                                    <br />
                                    <asp:TextBox runat="server" ID="passwordUserText" Width="120" Style="text-align: center; margin-left: 10px; margin-right: 10px; margin-bottom: 10px" />
                                    <br />
                                    <asp:Label runat="server" ID="errorValidation" Text="Access denied" ForeColor="Red" Style="width: 80%; margin: 0 auto; visibility: hidden" />
                                </div>

                                <div id="buttons">
                                    <div id="DivbtnOK" style="width:90%; margin: 0 auto">                                        
                                        <asp:Button ID="btnLogIn" runat="server" Text="Log in" OnClick="btnLogIn_Click" Style="margin-bottom: 10px; border-radius: 5px" />
                                        <asp:Button ID="btnCancel1" runat="server" Text="Cancel" Style="margin-bottom: 10px; border-radius: 5px" OnClick="btnCancel1_Click" />
                                    </div>
                                </div>
                            </div>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
