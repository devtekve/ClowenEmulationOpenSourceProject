using CLFramework;
namespace CLGameServer.Client
{
    public class Parse
    {
		public static void OperationCodes(Decode de)
        {
            try
            {
                PlayerMgr sys = (PlayerMgr)de.Packet;
                sys.PacketInformation = de;
                switch (de.opcode)
                {
                    case OperationCode.CLIENT_PING:
                    case OperationCode.CLIENT_PING2:
                        break;
                    case OperationCode.CLIENT_PATCH:
                        sys.Patch();
                        break;
                    case OperationCode.CLIENT_CONNECTION:
                        sys.Connect();
                        break;
                    case OperationCode.CLIENT_CHARACTERSCREEN:
                        sys.CharacterScreen();
                        break;
                    case OperationCode.CLIENT_INGAME_REQUEST:
                        sys.LoginScreen();
                        break;
                    case OperationCode.CLIENT_INGAME_SUCCESS:
                        sys.InGameSuccess();
                        break;
                    case OperationCode.CLIENT_REQUEST_WEATHER:
                        sys.LoadWeather();
                        break;
                    case OperationCode.CLIENT_SIT:
                        sys.Doaction();
                        break;
                    case OperationCode.CLIENT_QUESTMARK:
                        sys.QuestionMark();
                        break;
                    case OperationCode.CLIENT_MOVEMENT:
                        sys.Movement();
                        break;
                    case OperationCode.CLIENT_ANGLE_MOVE:
                        sys.Angle();
                        break;
                    case OperationCode.CLIENT_SAVE_BAR:
                        sys.Save();
                        break;
                    case OperationCode.CLIENT_LEAVE_REQUEST:
                        sys.LeaveGame();
                        break;
                    case OperationCode.CLIENT_LEAVE_CANCEL:
                        sys.CancelLeaveGame();
                        break;
                    case OperationCode.CLIENT_ITEM_MOVE:
                        sys.ItemMain();
                        break;
                    case OperationCode.CLIENT_SELECT_OBJECT:
                        sys.SelectObject();
                        break;
                    case OperationCode.CLIENT_GM:
                        sys.GameMaster();
                        break;
                    case OperationCode.CLIENT_EMOTE:
                        sys.Emote();
                        break;
                    case OperationCode.CLIENT_TELEPORTSTART:
                        sys.Teleport_Start();
                        break;
                    case OperationCode.CLIENT_TELEPORTDATA:
                        sys.Teleport_Data();
                        break;
                    case OperationCode.CLIENT_CHAT:
                        sys.Chat();
                        break;
                    case OperationCode.CLIENT_MAINACTION:
                        sys.ActionMain();
                        break;
                    case OperationCode.CLIENT_MASTERY_UP:
                        sys.Mastery_Up();
                        break;
                    case OperationCode.CLIENT_SKILL_UP:
                        sys.Mastery_Skill_Up();
                        break;
                    case OperationCode.CLIENT_GETUP:
                        sys.Player_Up();
                        break;
                    case OperationCode.CLIENT_REQUEST_PARTY:
                        sys.NormalRequest();
                        break;
                    case OperationCode.CLIENT_PARTY_REQUEST:
                        sys.CharacterRequest();
                        break;
                    case OperationCode.CLIENT_EXCHANGE_REQUEST:
                        sys.Exchange_Request();
                        break;
                    case OperationCode.CLIENT_EXCHANGE_WINDOWS_CLOSE:
                        sys.Exchange_Close();
                        break;
                    case OperationCode.CLIENT_EXCHANGE_ACCEPT:
                        sys.Exchange_Accept();
                        break;
                    case OperationCode.CLIENT_EXCHANGE_APPROVE:
                        sys.Exchange_Approve();
                        break;
                    case OperationCode.CLIENT_ACADEMY_MATCHING_LIST:
                        //sys.Send(Packet.ListAcademyMatching(Helpers.Manager.Party));
                        break;
                    case OperationCode.CLIENT_PARTY_ADDMEMBERS:
                        sys.PartyAddmembers();
                        break;
                    case OperationCode.CLIENT_PARTY_LEAVE:
                        sys.LeaveParty();
                        break;
                    case OperationCode.CLIENT_PARTY_BANPLAYER:
                        sys.PartyBan();
                        break;
                    case OperationCode.CLIENT_GUIDE:
                        sys.Gameguide();
                        break;
                    case OperationCode.CLIENT_PLAYER_UPDATE_INT:
                        sys.InsertInt();
                        break;
                    case OperationCode.CLIENT_PLAYER_UPDATE_STR:
                        sys.InsertStr();
                        break;
                    case OperationCode.CLIENT_PLAYER_HANDLE:
                        sys.Handle();
                        break;
                    case OperationCode.CLIENT_PLAYER_BERSERK:
                        sys.Player_Berserk_Up();
                        break;
                    case OperationCode.CLIENT_CLOSE_NPC:
                        sys.Close_NPC();
                        break;
                    case OperationCode.CLIENT_OPEN_NPC:
                        sys.Open_NPC();
                        break;
                    case OperationCode.CLIENT_NPC_BUYPACK:
                        sys.Player_BuyPack();
                        break;
                    case OperationCode.CLIENT_OPEN_WAREHOUSE:
                        sys.Open_Warehouse();
                        break;
                    case OperationCode.CLIENT_CLOSE_SCROLL:
                        sys.StopScrollTimer();
                        break;
                    case OperationCode.CLIENT_SAVE_PLACE:
                        sys.SavePlace();
                        break;
                    case OperationCode.CLIENT_ALCHEMY:
                        sys.AlchemyElixirMain();
                        break;
                    case OperationCode.CLIENT_ALCHEMY_CREATE_STONE:
                        sys.AlchemyCreateStone();
                        break;
                    case OperationCode.CLIENT_PET_MOVEMENT:
                        sys.MovementPet();
                        break;
                    case OperationCode.CLIENT_PET_TERMINATE:
                        sys.HandleClosePet();
                        break;
                    case OperationCode.CLIENT_START_PK:
                        sys.PkPlayer();
                        break;
                    case OperationCode.CLIENT_PARTYMATCHING_LIST_REQUEST:
                        sys.ListPartyMatching(Helpers.Manager.Party);
                        break;
                    case OperationCode.CLIENT_CREATE_FORMED_PARTY:
                        sys.CreateFormedParty();
                        break;
                    case OperationCode.CLIENT_FORMED_PARTY_DELETE:
                        sys.DeleteFormedParty(0);
                        break;
                    case OperationCode.CLIENT_JOIN_FORMED_RESPONSE:
                        sys.FormedResponse();
                        break;
                    case OperationCode.CLIENT_CHANGE_PARTY_NAME:
                        sys.RenameParty();
                        break;
                    case OperationCode.CLIENT_JOIN_FORMED_PARTY:
                        sys.JoinFormedParty();
                        break;
                    case OperationCode.CLIENT_GUILD:
                        sys.GuildCreate();
                        break;
                    case OperationCode.CLIENT_GUILD_TRANSFER:
                        sys.GuildTransferLeaderShip();
                        break;
                    case OperationCode.CLIENT_GUILD_PERMISSIONS:
                        sys.GuildPermissions();
                        break;
                    case OperationCode.CLIENT_GUILD_PROMOTE:
                        sys.GuildPromote();
                        break;
                    case OperationCode.CLIENT_GUILD_DISBAND:
                        sys.GuildDisband();
                        break;
                    case OperationCode.CLIENT_GUILD_MESSAGE:
                        sys.GuildMessage();
                        break;
                    case OperationCode.CLIENT_OPEN_GUILD_STORAGE:
                        sys.GuildStorage();
                        break;
                    case OperationCode.CLIENT_CLOSE_GUILD_STORAGE:
                        sys.GuildStorageClose();
                        break;
                    case OperationCode.CLIENT_GUILD_WAR_GOLD:
                        sys.GuildWarGold();
                        break;
                    case OperationCode.CLIENT_OPEN_GUILD_STORAGE2:
                        sys.GuildStorage2();
                        break;
                    case OperationCode.CLIENT_GUILD_KICK:
                        sys.KickFromGuild();
                        break;
                    case OperationCode.CLIENT_GUILD_LEAVE:
                        sys.GuildLeave();
                        break;
                    case OperationCode.CLIENT_GUILD_TITLE_SET:
                        sys.GuildTitle();
                        break;
                    case OperationCode.CLIENT_GUILD_INVITE:
                        sys.GuildInvite();
                        break;
                    case OperationCode.CLIENT_GUILD_DONATE_GP:
                        sys.DonateGP();
                        break;
                    case OperationCode.CLIENT_GACHA_PLAY:
                        //Add function
                        break;
                    case OperationCode.CLIENT_JOIN_MERC:
                        sys.JoinMerc();
                        break;
                    case OperationCode.CLIENT_RANKING_LISTS:
                        sys.RankList();
                        break;
                    case OperationCode.CLIENT_PREV_JOB:
                        sys.PrevJob();
                        break;
                    case OperationCode.CLIENT_HONOR_RANK:
                        sys.HonorRank();
                        break;
                    case OperationCode.CLIENT_PM_MESSAGE:
                        sys.PrivateMessage();
                        break;
                    case OperationCode.CLIENT_PM_SEND:
                        sys.PrivateMessageSend();
                        break;
                    case OperationCode.CLIENT_PM_OPEN:
                        sys.PrivateMessageOpen();
                        break;
                    case OperationCode.CLIENT_PM_DELETE:
                        sys.PrivateMessageDelete();
                        break;
                    case OperationCode.CLIENT_PET_UNSUMMON:
                        sys.UnSummonPet();
                        break;
                    case OperationCode.CLIENT_PET_RENAME:
                        sys.RenamePet();
                        break;
                    case OperationCode.CLIENT_GPET_SETTINGS:
                        sys.GrabPetSettings();
                        break;
                    case OperationCode.CLIENT_MAKE_ALIAS:
                        sys.MakeAlias();
                        break;
                    case OperationCode.CLIENT_LEAVE_JOB:
                        sys.LeaveJob();
                        break;
                    case OperationCode.CLIENT_DISSEMBLE_ITEM:
                        sys.BreakItem();
                        break;
                    case OperationCode.CLIENT_STALL_OPEN:
                        sys.StallOpen();
                        break;
                    case OperationCode.CLIENT_STALL_CLOSE:
                        sys.StallClose();
                        break;
                    case OperationCode.CLIENT_STALL_BUY:
                        sys.StallBuy();
                        break;
                    case OperationCode.CLIENT_STALL_ACTION:
                        sys.StallMain();
                        break;
                    case OperationCode.CLIENT_STALL_OTHER_OPEN:
                        sys.EnterStall();
                        break;
                    case OperationCode.CLIENT_STALL_OTHER_CLOSE:
                        sys.LeaveStall();
                        break;
                    case OperationCode.CLIENT_PVP:
                        sys.StartPvpTimer(10000);
                        break;
                    case OperationCode.CLIENT_ALCHEMY_STONE:
                        sys.AlchemyStoneMain();
                        break;
                    case OperationCode.CLIENT_ITEM_MALL_WEB:
                        sys.ItemMallWeb();
                        break;
                    case OperationCode.CLIENT_ITEM_STORAGE_BOX:
                        sys.ItemStorageBox();
                        break;
                    case OperationCode.CLIENT_ITEM_BOX_LOG:
                        sys.ItemStorageBoxLog();
                        break;
                    case OperationCode.CLIENT_FRIEND_REMOVAL:
                        sys.FriendRemoval();
                        break;
                    case OperationCode.CLIENT_FRIEND_INVITE:
                        sys.FriendAdd();
                        break;
                    case OperationCode.CLIENT_FRIEND_GROUP:
                        sys.FriendGroup("ADD");
                        break;
                    case OperationCode.CLIENT_FRIEND_GROUP_REMOVE:
                        sys.FriendGroup("REMOVE");
                        break;
                    case OperationCode.CLIENT_FRIEND_GROUP_MANAGE_FRIEND:
                        sys.FriendGroup("MOVE");
                        break;
                    case OperationCode.CLIENT_FRIEND_INVITE_RESPONSE:
                        sys.FriendAddResponse();
                        break;
                    case OperationCode.CLIENT_UNION_APPLY:
                        sys.unionapply();
                        break;
                    case OperationCode.CLIENT_ICON_REQUEST:
                        sys.RequestIcons();
                        break;
                    default:
                        Print.Format("(0x{0}) {1}", de.opcode.ToString("X4"), Decode.StringToPack(sys.PacketInformation.buffer));
                        break;
                }
                sys.Dispose();
                sys = null;
            }
            catch (System.Exception ex)
            {
                Log.Exception(ex);
            }
        }
	}
}